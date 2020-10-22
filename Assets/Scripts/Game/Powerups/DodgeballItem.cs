using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DodgeballItem : PowerUp
{
    private GameObject ball;

    public override void Use()
    {
        ball = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Trefbal"), player.throwPos.position, player.throwPos.rotation);
        ball.GetComponent<Dodgeball>().thrower = player;
        ball.GetComponent<Rigidbody>().AddForce(player.transform.forward * player.forwardThrowForce + player.transform.up * player.upwardsThrowForce);
        base.Use();
    }
}
