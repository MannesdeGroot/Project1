using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeballItem : PowerUp
{
    [SerializeField] private GameObject dodgeball;

    public override void Use()
    {
        GameObject ball = Instantiate(dodgeball, player.throwPos.position, player.throwPos.rotation);
        ball.GetComponent<Rigidbody>().AddForce(player.transform.forward * player.throwForce);
    }
}
