using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public string userName;
    public int score;

    [Header("Tag")]
    public bool isTagger;
    [SerializeField] private Color taggerColor;
    [SerializeField] private Color runnerColor;
    [SerializeField] private Text roleText;

    public void SetTagger(bool value)
    {
        isTagger = value;

        if (roleText == null) return;
        roleText.color = isTagger ? taggerColor : runnerColor;
        roleText.text = isTagger ? "Tagger" : "Runner";
    }
}
