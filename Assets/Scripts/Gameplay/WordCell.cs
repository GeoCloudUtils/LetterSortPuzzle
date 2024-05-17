using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordCell : MonoBehaviour
{
    [SerializeField] private TMP_Text letterText;

    public void SetLetter(string letter, bool isHidden = true)
    {
        letterText.SetText(isHidden ? "?" : letter);
    }

    public void Activate()
    {
        letterText.enabled = true;
    }
}
