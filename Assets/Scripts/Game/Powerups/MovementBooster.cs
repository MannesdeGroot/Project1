using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBooster : MonoBehaviour
{
    [SerializeField] private BoosterType type;
    [SerializeField] private float multiplier;
    [SerializeField] private float duration;

    public void OnTriggerEnter(Collider c)
    {
        Movement player = c.GetComponent<Movement>();

        if (player != null)
        {
            StartCoroutine(player.ApplyBoost(type, multiplier, duration));
            Activate(false);
        }
    }

    private void Activate(bool active)
    {
        gameObject.GetComponent<Collider>().enabled = active;
        gameObject.GetComponent<Renderer>().enabled = active;
    }
}
