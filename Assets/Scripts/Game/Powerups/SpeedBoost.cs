using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : PowerUp
{
    [SerializeField] private float multiplier;
    [SerializeField] private float duration;

    public override void Use()
    {
        player.ApplySpeedBoost(multiplier, duration);
        base.Use();
    }
}
