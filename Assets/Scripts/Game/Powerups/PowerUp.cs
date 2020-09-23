using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public abstract class PowerUp : MonoBehaviour, Photon.Pun.IPunObservable
{
    protected PlayerController player;
    public Sprite icon;
    public string powerUpName;
    PhotonView pv;
    public float rotateSpeed;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    public abstract void Use();

    private void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
    public void OnTriggerEnter(Collider c)
    {
        PlayerController _player = c.GetComponent<PlayerController>();
        if (_player == null) return;

        if(_player.powerUp == null)
        {
            _player.AddPowerUp(this);
            player = _player;
            pv.RPC("ActivateSelf", RpcTarget.All, false);
        }
    }

    [PunRPC]
    public void ActivateSelf(bool value)
    {
        GetComponent<Renderer>().enabled = value;
        GetComponent<Collider>().enabled = value;
        if (!value)
        {
            Destroy(gameObject, 5);
        }
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
