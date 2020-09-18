using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourManager : MonoBehaviour
{
    private bool gameEnded;

    private void EndGame(PlayerController winner)
    {
        print($"{winner} won");
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
