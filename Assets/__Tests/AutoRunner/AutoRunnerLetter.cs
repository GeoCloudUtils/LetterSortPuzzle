using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutoRunnerLetter : MonoBehaviour
{
    public TMP_Text text;

    public string Letter { get; private set; }

    public void SetLetter(string letter)
    {
        text.text = letter;
        Letter = letter;
    }
}
