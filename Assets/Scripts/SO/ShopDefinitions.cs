using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop Definition", menuName = "ScriptableObjects/Create Shop Defintion", order = 1)]
public class ShopDefinitions : ScriptableObject
{
    [SerializeField] private ShopDefinition[] shopDefinitions;

    public ShopDefinition[] ShopDef { get => shopDefinitions; private set => shopDefinitions = value; }
}

[Serializable]
public class ShopDefinition
{
    public int Level;
    public Sprite Sprite;
    public int Price;
}
