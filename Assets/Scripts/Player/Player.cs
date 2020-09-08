using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public string userName;
    public int score;
    private float timer;
    private PowerUp powerUp;
    [SerializeField] private Image powerUpIcon;
    [SerializeField] private Text powerUpName;

    [Header("Tag")]
    public bool isTagger;
    [SerializeField] private Color taggerColor;
    [SerializeField] private Color runnerColor;
    [SerializeField] private Text roleText;
    [SerializeField] private Text timerText;

    public void SetTagger(bool value)
    {
        isTagger = value;

        if (roleText == null) return;
        roleText.color = isTagger ? taggerColor : runnerColor;
        roleText.text = isTagger ? "Tagger" : "Runner";

        timerText.gameObject.SetActive(isTagger);

        if (isTagger)
        {
            timer = GameSettings.eliminationTime;
            StartCoroutine(CountDown());
        }
        else
        {
            StopCoroutine(CountDown());
        }
    }

    private void EliminatePlayer()
    {

    }

    private IEnumerator CountDown()
    {
        if (timer <= 0)
            EliminatePlayer();

        if (timer >= 0)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            string secondsText = seconds < 10 ? $"0{seconds}" : seconds.ToString();
            timerText.text = $"{minutes}:{secondsText}";
        }

        yield return new WaitForSeconds(1);
        timer--;

        StartCoroutine(CountDown());
    }

    public float GetTagKnockBack()
    {
        return GameSettings.tagKnockBack * (GameSettings.eliminationTime / timer);
    }
}
