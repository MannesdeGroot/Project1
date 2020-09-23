using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourManager : MonoBehaviour
{
    private bool gameEnded;
    public GameObject voteCam;
    public VoteSystem voteSystem;

    private void EndGame(PlayerController winner)
    {
        foreach (PlayerController player in FindObjectsOfType<PlayerController>())
        {
            print(1);
            Destroy(player.gameObject);
            print($"{winner} won");
            voteCam.SetActive(true);
            voteSystem.PhotonStartVoting();
        }

    }

    private void OnTriggerEnter(Collider c)
    {
        PlayerController player = c.GetComponent<PlayerController>();

        if (player != null && !gameEnded)
        {
            EndGame(player);
        }
    }
}
