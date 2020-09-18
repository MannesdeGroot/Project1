using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeballItem : PowerUp
{
    [SerializeField] private GameObject dodgeball;
    [SerializeField] private float throwAnimTime;

    public override void Use() => StartCoroutine(Throw());

    private IEnumerator Throw()
    {
        GameObject ball = Instantiate(dodgeball, player.throwPos.position, player.throwPos.rotation);
        ball.transform.parent = player.throwPos;
        ball.GetComponent<Dodgeball>().thrower = player;

        yield return new WaitForSeconds(throwAnimTime);
        ball.transform.parent = null;
        ball.GetComponent<Rigidbody>().useGravity = true;
        ball.GetComponent<Rigidbody>().AddForce(player.transform.forward * player.forwardThrowForce + player.transform.up * player.upwardsThrowForce);
        player.animThrow = false;
    }
}
