using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blanket : PowerUp
{
    public float duration;

    public override void Use()
    {
        StartCoroutine(player.SetInvincible(duration));
    }
}
