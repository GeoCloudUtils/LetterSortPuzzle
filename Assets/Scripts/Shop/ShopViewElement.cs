using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopViewElement : MonoBehaviour
{
    [SerializeField] private ShopDefinitions shopDefintion;
    [SerializeField] private ClickableShopBox boxPrefab;
    [SerializeField] private Transform parent;

    public void Init()
    {
        foreach (ShopDefinition def in shopDefintion.ShopDef)
        {
            ClickableShopBox box = Instantiate(boxPrefab, parent);
            box.Initialize(def.Price, def.Level, def.Sprite);
            box.OnBuy += HandleShopItemBuy;
        }
    }

    private void HandleShopItemBuy(int price)
    {
        if (CanBuy(price))
        {
            //proccess buy
        }
        else
        {
            Debug.Log("Not enought coins!");
        }
    }

    private bool CanBuy(int price)
    {
        int coins = PlayerPrefs.GetInt("COINS", 0);
        return coins >= price;
    }
}
