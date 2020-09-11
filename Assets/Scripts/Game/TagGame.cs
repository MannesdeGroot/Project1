using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TagGame : EliminationGame, Photon.Pun.IPunObservable
{
    public int taggersAmount;
    private List<PlayerController> taggers;

    public override void StartGame()
    {
        base.StartGame();

        foreach(PlayerController player in players)
        {
            player.isTagger = false;
            player.SetTagger(false);
        }

        taggers = new List<PlayerController>();

        for (int i = 0; i < taggersAmount; i++)
        {
            int random = Random.Range(0, players.Count);

            PlayerController player = players[random];
            if (!taggers.Contains(player))
            {
                taggers.Add(player);
                player.PhotonTag(0);
            }
            else
            {
                i--;
            }
        }
    }
    
    public void TagPlayer(PlayerController tagger, PlayerController tagged)
    {
        if (tagged.isTagger) return;

        taggers.Remove(tagger);
        tagger.SetTagger(false);

        taggers.Add(tagged);
        tagged.SetTagger(true);
    }

    protected override void LoadSettings()
    {
        base.LoadSettings();

        taggersAmount = GameSettings.amountOfTaggers;
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(taggers);
        }
        else if (stream.IsReading)
        {
            taggers = (List<PlayerController>)stream.ReceiveNext();
        }
    }
}
