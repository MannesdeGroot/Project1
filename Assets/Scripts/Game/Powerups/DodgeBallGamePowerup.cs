using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DodgeBallGamePowerup : PowerUp
{

    public override void Use()
    {
        GameObject ball = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "DodgeballGame"), player.throwPos.position, player.throwPos.rotation);
        ball.GetComponent<DodgeBall>().SetTeam(player.team);
    }
}
