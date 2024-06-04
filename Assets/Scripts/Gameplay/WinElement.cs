using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinElement : MonoBehaviour
{

    [SerializeField] private TMP_Text countText;

    public void SetWinCount(int count)
    {
        countText.SetText("+ " + count.ToString());
    }
}
