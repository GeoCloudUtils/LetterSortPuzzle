using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Backgrounds Definition", menuName = "ScriptableObjects/Create Backgrounds Defintion", order = 1)]
public class Backgrounds : ScriptableObject
{
    [SerializeField] private BackgroundItem[] backrounds;

    public BackgroundItem GetBackgroundByName(string name)
    {
        BackgroundItem item = backrounds.ToList().Find(e => e.background.name == name);
        return item;
    }
}

[Serializable]
public class BackgroundItem
{
    public Sprite background;
    public Color bodyColor;
}
