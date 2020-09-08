using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBoost : PowerUp
{
    public override void Use()
    {
        player.powerJumps++;
    }
}
