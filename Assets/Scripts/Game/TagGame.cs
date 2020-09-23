using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TagGame : EliminationGame, Photon.Pun.IPunObservable
{
    public int taggersAmount;
    private List<string> taggers;
    PhotonView pV;


    private void Start()
    {
        pV = transform.GetComponent<PhotonView>();
    }
    public override void StartGame()
    {
        base.StartGame();

        foreach(GameObject player in players)
        {
            player.GetComponent<PlayerController>().isTagger = false;
            player.GetComponent<PlayerController>().SetTagger(false);
        }

        taggers = new List<string>();
        List<string> playerIDs = new List<string>();
        for (int i = 0; i < players.Count; i++)
        {
            playerIDs.Add(players[i].GetComponent<PhotonView>().ViewID.ToString());
        }

        for (int i = 0; i < taggersAmount; i++)
        {
            int random = Random.Range(0, playerIDs.Count);

            //PlayerController player = players[random].GetComponent<PlayerController>();
            string playerID = playerIDs[random];
            if (!taggers.Contains(playerID))
            {
                for (int j = 0; j < players.Count; j++)
                {
                    if(players[j].GetComponent<PhotonView>().ViewID.ToString() == playerID)
                    {
                        taggers.Add(playerID);
                        players[j].GetComponent<PlayerController>().PhotonTag(transform.position, 0);
                    }
                }
                
            }
            else
            {
                i--;
            }
        }
    }

    

    public void TagPlayer(string taggerID, string taggedID)
    {
        print(taggerID + "  " + taggedID);
        if (taggers.Contains(taggerID)) return;

        taggers.Remove(taggerID);

        taggers.Add(taggedID);
    }

    protected override void LoadSettings()
    {
        base.LoadSettings();

        taggersAmount = GameSettings.amountOfTaggers;
    }

    
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(taggers);
        }
        else if (stream.IsReading)
        {
            //taggers = (List<GameObject>)stream.ReceiveNext();
        }
    }
}
