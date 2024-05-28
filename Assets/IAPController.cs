using GoogleMobileAds.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using Task = System.Threading.Tasks.Task;
using UnityEngine.Purchasing.Security;
using static UnityEngine.Networking.UnityWebRequest;
using System.Text;
using System.Security.Cryptography;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

[Serializable]
public class IAPProductDefinition
{
    public string id;
    public ProductType type;
}

public enum PurchaseState
{
    Processing = 0,
    Success = 1,
    Failed = 2
}

public class IAPController: MonoBehaviour, IDetailedStoreListener
{
    private class Transaction
    {
        public string productId;
        // public Action<bool> statusCallback;
        public PurchaseState state;
        // public bool processed;
        public bool invalidReceipt;
        public bool pending;
    }

    [Header("General")]
    [SerializeField] private bool _testMode = true;
    [SerializeField] private List<IAPProductDefinition> _products;
    private bool? _initialized;
    private bool _useAppleStoreKitTestCertificate;
    private IStoreController _controller;
    private IExtensionProvider _extensions;
    private Transaction _currentTransaction;
    private CrossPlatformValidator _validator;
    private ConfigurationBuilder _config;

    private async void Awake()
    {
        await Init();
    }

    private async Task Init()
    {
        string environment = "dev";
#if PROD_BUILD
        environment = "prod";
#endif
        try
        {
            var options = new InitializationOptions().SetEnvironmentName(environment);

            await UnityServices.InitializeAsync(options);
        }
        catch (Exception exception)
        {
            Debug.LogError($"[IAPController] NOT Initialized. Reason: UnityServices failed to init. Error:{exception.Message}");
            _initialized = false;
            return;
        }

        // StoreKit Testing in Xcode is a local test environment for testing in-app purchases without requiring a connection to App Store servers. 
        _useAppleStoreKitTestCertificate = _testMode;

        _config = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach(var elem in _products)
        {
            _config.AddProduct(elem.id, elem.type);
        }

        UnityPurchasing.Initialize(this, _config);

        while (!_initialized.HasValue)
        {
            await Task.Yield();
        }

        if (_initialized.HasValue && _initialized.Value == true)
        {
            InitValidator();
        }
    }

    /// <summary>
    /// Initialize Validator: Validator to validate Purchase Receipts.
    /// </summary>
    private void InitValidator()
    {
        if (IsCurrentStoreSupportedByValidator())
        {
#if !UNITY_EDITOR
                var appleTangleData = _useAppleStoreKitTestCertificate ? AppleStoreKitTestTangle.Data() : AppleTangle.Data();
                _validator = new CrossPlatformValidator(GooglePlayTangle.Data(), appleTangleData, Application.identifier);
#endif
        }
        else
        {
            Debug.LogError($"[IAPController] Validator NOT initialized, store not supported. Store:{GetStore()}");
        }
    }

    public async Task<PurchaseState> Purchase(string productId)
    {
        if (_initialized == null)
        {
            Debug.LogError($"[IAPController] Not initialized.");
            return PurchaseState.Failed;
        }
        else if(_initialized.HasValue && _initialized.Value == false)
        {
            Debug.LogError($"[IAPController] Not initialized(failed the intialization).");
            return PurchaseState.Failed;
        }
        else if (!CanPurchase())
        {
            Debug.LogError($"[IAPController] Purchasing not available([SKPaymentQueue canMakePayments]).");
            return PurchaseState.Failed;
        }
        else if (_currentTransaction != null)
        {
            Debug.LogError($"[IAPController] A transaction already in progress. Product:{_currentTransaction.productId}");
            return PurchaseState.Failed;
        }

        _currentTransaction = new Transaction()
        {
            productId = productId,
            state = PurchaseState.Processing
        };

        _controller.InitiatePurchase(productId);

        while (_currentTransaction.state == PurchaseState.Processing)
        {
            await Task.Yield();
        }

        PurchaseState result =  _currentTransaction.state;

        _currentTransaction = null;

        return result;
    }

    public async Task<bool> RestoreTransactions()
    {
        bool? result = null;

        if (IsAppleStore())
        {
            var extensions = _extensions.GetExtension<IAppleExtensions>();
            extensions.RestoreTransactions((bool success, string error) =>
            {
                if (success)
                {
                    Debug.Log($"[IAPController] Purchases restored.");
                    result = true;
                }
                else
                {
                    Debug.LogError($"[IAPController] Purchases NOT restored. Error:{error}");
                    result = false;
                }
            });
        }
        else if (IsGoogleStore())
        {
            var extensions = _extensions.GetExtension<IGooglePlayStoreExtensions>();
            extensions.RestoreTransactions((bool success, string error) =>
            {
                if (success)
                {
                    Debug.Log($"[IAPController] Purchases restored.");
                    result = true;
                }
                else
                {
                    Debug.LogError($"[IAPController] Purchases NOT restored. Error:{error}");
                    result = false;
                }
            });
        }
        else
        {
            Debug.LogError("[IAPController] Unsupported Store to Restore. Store: " + GetStore());
            result = false;
        }

        while (!result.HasValue)
        {
            await Task.Yield();
        }
        return result.Value;
    }

    private bool CanPurchase()
    {
        if(IsAppleStore())
        {
            return _config.Configure<IAppleConfiguration>().canMakePayments;
        }
        return true;
    }

    private bool IsCurrentStoreSupportedByValidator()
    {
        return IsAppleStore() || IsGoogleStore();
    }

    private bool ValidatePurchase(Product product)
    {
        // If we the validator doesn't support the current store, we assume the purchase is valid
        if (IsCurrentStoreSupportedByValidator() && _validator != null)
        {
            try
            {
                // The validator returns parsed receipts.
                IPurchaseReceipt[] receipts = _validator.Validate(product.receipt);
                //LogReceipts(receipts);
            }

            //If the purchase is deemed invalid, the validator throws an IAPSecurityException.
            catch (IAPSecurityException reason)
            {
                Debug.Log($"Invalid receipt: {reason}");
                return false;
            }
        }

        return true;
    }

    private bool IsAppleStore()
    {
        return GetStore() == AppStore.AppleAppStore;
    }

    private bool IsGoogleStore()
    {
        return GetStore() == AppStore.GooglePlay;
    }

    private AppStore GetStore()
    {
        return StandardPurchasingModule.Instance().appStore;
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("[IAPController] Initialized.");
        _controller = controller;
        _extensions = extensions;
        _initialized = true;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"[IAPController] NOT Initialized. Error:{error}");
        _initialized = false;
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"[IAPController] NOT Initialized. Error:{error}; Message:{message}");
        _initialized = false;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError($"[IAPController] Purchase Failed. Product:{product.definition.id}; Error:{failureDescription.message}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"[IAPController] Purchase Failed. Product:{product.definition.id}; Error:{failureReason}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;

        bool isCurrTransaction = _currentTransaction != null && product.definition.id == _currentTransaction.productId;

        if (!ValidatePurchase(product))
        {
            Debug.LogError($"[IAPController] Purchase Failed. Reason: Invalid Receipt. Product:{product.definition.id};");
            if (isCurrTransaction)
            {
                _currentTransaction.invalidReceipt = true;
                _currentTransaction.state = PurchaseState.Failed;
            }
            // Return Pending, informing IAP that the processing on our side is NOT done yet.
            return PurchaseProcessingResult.Pending;
        }

        if (IsGoogleStore())
        {
            var extensions = _extensions.GetExtension<IGooglePlayStoreExtensions>();
            if (extensions.IsPurchasedProductDeferred(product))
            {
                // The purchase is Deferred. Therefore, we do not complete the transaction. ProcessPurchase will be called again once the purchase is Purchased.
                Debug.LogError($"[IAPController] Purchase Pending. Reason: Deferred Purchase. Product:{product.definition.id};");
                if (isCurrTransaction)
                {
                    _currentTransaction.pending = true;
                    _currentTransaction.state = PurchaseState.Failed;
                }
                // Return Pending, informing IAP that the processing on our side is NOT done yet.
                return PurchaseProcessingResult.Pending;
            }
        }

        if(isCurrTransaction)
            Debug.Log($"[IAPController] Purchase Success. Product:{product.definition.id};");
        else
            Debug.Log($"[IAPController] Purchase Success(but not current transaction, maybe diferred purchase), accepting as is. Product:{product.definition.id}");

        _currentTransaction.state = PurchaseState.Success;

        // Return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
        return PurchaseProcessingResult.Complete;
    }
}
