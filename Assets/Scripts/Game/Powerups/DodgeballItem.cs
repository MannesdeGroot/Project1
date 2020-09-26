using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DodgeballItem : PowerUp
{
    [SerializeField] private float throwAnimTime;
    private GameObject ball;

    public override void Use() => StartCoroutine(Throw());

    private IEnumerator Throw()
    {
        pv.RPC("InstantiateBall", RpcTarget.All);
        yield return new WaitForSeconds(throwAnimTime);
        pv.RPC("ThrowBall", RpcTarget.All);
    }

    [PunRPC]
    public void InstantiateBall()
    {
        ball = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Trefbal"), player.throwPos.position, player.throwPos.rotation);
        ball.transform.parent = player.throwPos;
        ball.GetComponent<Dodgeball>().thrower = player;
    }

    [PunRPC]
    public void ThrowBall()
    {
        ball.transform.parent = null;
        ball.GetComponent<Rigidbody>().useGravity = true;
        ball.GetComponent<Rigidbody>().AddForce(player.transform.forward * player.forwardThrowForce + player.transform.up * player.upwardsThrowForce);
        player.animThrow = false;
    }
}
