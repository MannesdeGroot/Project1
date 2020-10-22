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
    public GameObject pickUpSound;
    public GameObject useSound;

    public void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    public virtual void Use()
    {
        if(player.pV.IsMine && useSound != null)
        {
            Instantiate(useSound, player.transform.position, Quaternion.identity);
            //pv.RPC("PlayUseSound", RpcTarget.All);
        }
        Destroy(gameObject, 5);
    }

    [PunRPC]
    void PlayUseSound()
    {
        Instantiate(useSound, player.transform.position, Quaternion.identity);

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

            if (player.pV.IsMine && pickUpSound != null)
            {
                Instantiate(pickUpSound, player.transform.position, Quaternion.identity);
            }
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
