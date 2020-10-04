using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodgeball : MonoBehaviour, IPunObservable
{
    public PlayerController thrower;
    [SerializeField] private bool active;
    [SerializeField] private float duration;
    [SerializeField] private float despawnTime;
    private PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    private void OnCollisionEnter(Collision c)
    {
        if (!active) return;

        PlayerController player = c.gameObject.GetComponent<PlayerController>();

        if (player != null && player != thrower)
        {
            StartCoroutine(player.Stun(duration));
            active = false;
            pv.RPC("DestroySelf", RpcTarget.All);
        }
    }

    [PunRPC]
    public void DestroySelf()
    {
        Destroy(gameObject, despawnTime);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

        }
        if (stream.IsReading)
        {

        }
    }
}
