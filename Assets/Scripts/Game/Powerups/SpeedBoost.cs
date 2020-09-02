using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour
{
    [SerializeField] private float multiplier;
    [SerializeField] private float duration;

    private void OnTriggerEnter(Collider c)
    {
        Movement player = c.GetComponent<Movement>();
        if (player == null) return;

        player.ApplySpeedBoost(multiplier, duration);

        Destroy(gameObject);
    }
}
