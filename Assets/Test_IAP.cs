using UnityEngine;
using UnityEngine.UI;

public class Test_IAP: MonoBehaviour
{
    [SerializeField] private IAPController _controller;
    [SerializeField] private Button _buyConsumable;
    [SerializeField] private Button _buyNonConsumable;
    [SerializeField] private string _consumableProductId;
    [SerializeField] private string _nonConsumableProductId;

    private void Start()
    {
        _buyConsumable.onClick.AddListener(OnBuyConsumableClicked);
        _buyNonConsumable.onClick.AddListener(OnBuyNonConsumableClicked);
    }

    private async void OnBuyConsumableClicked()
    {
        await _controller.Purchase(_consumableProductId);
    }

    private async void OnBuyNonConsumableClicked()
    {
        await _controller.Purchase(_nonConsumableProductId);
    }
}
