using Lean.Gui;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ShopItemState
{
    LOCKED = 0,
    UNLOCKED = 1
}

public class ClickableShopBox : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private LeanButton selectButton;
    [SerializeField] private GameObject selectedIndicator;
    [SerializeField] private Image boxImage;
    [SerializeField] private LeanButton buyButton;
    [SerializeField] private GameObject lockedStateIndicator;
    [SerializeField] private GameObject unlockedStateIndicator;

    public event Action<int> OnBuy;

    private int itemPrice;
    private ShopItemState currentState = ShopItemState.LOCKED;

    private void Start()
    {
        buyButton.OnClick.AddListener(BuyItem);
        selectButton.OnClick.AddListener(SelectItem);
    }

    public void Initialize(int price, int level, Sprite itemSprite)
    {
        boxImage ??= GetComponent<Image>();
        levelText.SetText($"Level {level}");
        boxImage.sprite = itemSprite;
        itemPrice = price;

        TMP_Text priceText = buyButton.transform.Find("Cap/price")?.GetComponent<TMP_Text>();
        if (priceText == null)
        {
            Debug.LogWarning("Price text not found");
        }
        else
        {
            priceText.SetText(price.ToString());
        }

        int playerLevel = PlayerPrefs.GetInt("LEVEL", 0);
        bool isItemBought = PlayerPrefs.GetInt($"LEVEL{level}", 0) == 1;
        currentState = playerLevel >= level || isItemBought ? ShopItemState.UNLOCKED : ShopItemState.LOCKED;
        UpdateState(currentState);
        if(playerLevel == 0)
        {
            SelectItem();
        }
    }

    public void UpdateState(ShopItemState newState)
    {
        currentState = newState;
        lockedStateIndicator.SetActive(currentState == ShopItemState.LOCKED);
        unlockedStateIndicator.SetActive(currentState == ShopItemState.UNLOCKED);

        if (currentState == ShopItemState.UNLOCKED)
        {
            boxImage.color = Color.white;
        }
    }

    private void SelectItem()
    {
        if (currentState == ShopItemState.UNLOCKED)
        {
            selectButton.gameObject.SetActive(false);
            selectedIndicator.SetActive(true);
            PlayerPrefs.SetString("BG", boxImage.sprite.name);
        }
    }

    private void BuyItem()
    {
        OnBuy?.Invoke(itemPrice);
    }
}
