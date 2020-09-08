using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodgeball : MonoBehaviour
{
    [SerializeField] private bool active;
    [SerializeField] private float duration;
    [SerializeField] private float despawnTime;

    private void OnCollisionEnter(Collision c)
    {
        if (!active) return;

        PlayerController player = c.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            StartCoroutine(player.Stun(duration));
            active = false;
            Destroy(gameObject, despawnTime);
        }
    }
}
