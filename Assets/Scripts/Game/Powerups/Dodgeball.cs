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

        Movement player = c.gameObject.GetComponent<Movement>();

        if (player != null)
        {
            StartCoroutine(player.Stun(duration));
            active = false;
            Destroy(gameObject, despawnTime);
        }
    }
}
