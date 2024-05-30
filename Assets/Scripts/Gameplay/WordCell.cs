using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordCell : MonoBehaviour
{
    [SerializeField] private TMP_Text letterText;

    public bool IsShown { get; private set; } = false;
    public void SetLetter(string letter)
    {
        letterText.SetText(letter);
        letterText.gameObject.SetActive(false);
    }

    public void Show()
    {
        letterText.gameObject.SetActive(true);
        IsShown = true;
    }
}
