using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBoost : MonoBehaviour
{
    private void OnTriggerEnter(Collider c)
    {
        Movement player = c.GetComponent<Movement>();
        if (player == null) return;

        player.powerJumps++;
        Destroy(gameObject);
    }
}
