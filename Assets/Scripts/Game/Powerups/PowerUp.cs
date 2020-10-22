using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PowerUp : MonoBehaviour, Photon.Pun.IPunObservable
{
    protected PlayerController player;
    public Sprite icon;
    public string powerUpName;
    protected PhotonView pv;
    public float rotateSpeed;

    public void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    public virtual void Use()
    {
        Destroy(gameObject, 5);
    }

    private void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }

    public void OnTriggerEnter(Collider c)
    {
        PlayerController _player = c.GetComponent<PlayerController>();
        if (_player == null) return;

        if (_player.powerUp == null)
        {
            _player.AddPowerUp(this);
            player = _player;
            pv.RPC("ActivateSelf", RpcTarget.All, false);
        }
    }

    [PunRPC]
    public void ActivateSelf(bool value)
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(isTagger);

        }
        else if (stream.IsReading)
        {
            //isTagger = (bool)stream.ReceiveNext();

        }
    }
}
