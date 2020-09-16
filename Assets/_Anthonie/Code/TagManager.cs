using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TagManager : MonoBehaviour
{
    public float roundTime;
    public List<string> playerIDs = new List<string>();
    public PhotonView pV;
    public string[] playerIDsArray;
    private List<PlayerController> players = new List<PlayerController>();
    private float timer;

    void Start()
    {
        pV = transform.GetComponent<PhotonView>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartRound();
        }
    }
    
    public void ChooseTagger()
    {
        players = FindObjectsOfType<PlayerController>().ToList<PlayerController>();
        players[Random.Range(0, players.Count - 1)].PhotonTag(transform.position, 0);
    }

    private void StartRound()
    {
        ChooseTagger();
        timer = GameSettings.eliminationTime;
        StartCoroutine(CountDown());
    }

    private void EndRound()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].isTagger)
            {
                players[i].Eliminate();
                players.Remove(players[i]);
                break;
            }
        }

        if (players.Count > 1)
        {
            StartRound();
        }
        else
        {
            print($"{players[0]} won");
        }
    }

    private IEnumerator CountDown()
    {
        if (timer <= 0)
            EndRound();

        if (timer >= 0)
        {
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            string secondsText = seconds < 10 ? $"0{seconds}" : seconds.ToString();
            
            foreach (PlayerController player in players)
            {
                player.timerText.text = $"{minutes}:{secondsText}";
            }
        }

        yield return new WaitForSeconds(1);
        timer--;

        StartCoroutine(CountDown());
    }
}
