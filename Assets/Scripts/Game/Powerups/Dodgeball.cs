using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodgeball : MonoBehaviour
{
    public PlayerController thrower;
    [SerializeField] private bool active;
    [SerializeField] private float duration;
    [SerializeField] private float despawnTime;

    private void OnCollisionEnter(Collision c)
    {
        if (!active) return;

        PlayerController player = c.gameObject.GetComponent<PlayerController>();

        if (player != null && player != thrower)
        {
            StartCoroutine(player.Stun(duration));
            active = false;
            GetComponent<PhotonView>().RPC("DestroySelf", RpcTarget.All);
        }
    }

    [PunRPC]
    public void DestroySelf()
    {
        Destroy(gameObject, despawnTime);
    }
}
