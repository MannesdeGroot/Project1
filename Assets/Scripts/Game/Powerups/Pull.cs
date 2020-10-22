using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Pull : PowerUp
{
    public float range;
    public float knockbackPower;
    RaycastHit hit;
    public override void Use()
    {
        if (Physics.Raycast(player.cam.transform.position, player.cam.transform.forward, out hit, range))
        {
            if (hit.transform.tag == "Football")
            {
                hit.transform.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
                hit.transform.GetComponent<Rigidbody>().AddForce(-player.cam.transform.forward * knockbackPower);
            }
        }
    }
}
