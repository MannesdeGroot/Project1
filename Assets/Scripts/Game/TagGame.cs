using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TagGame : EliminationGame, Photon.Pun.IPunObservable
{
    public int taggersAmount;
    private List<GameObject> taggers;
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

        taggers = new List<GameObject>();

        for (int i = 0; i < taggersAmount; i++)
        {
            int random = Random.Range(0, players.Count);

            PlayerController player = players[random].GetComponent<PlayerController>();
            if (!taggers.Contains(player.gameObject))
            {
                taggers.Add(player.gameObject);
                player.PhotonTag(transform.position ,0);
            }
            else
            {
                i--;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartGame();
        }
    }

    public void TagPlayer(PlayerController tagger, PlayerController tagged)
    {
        if (tagged.isTagger) return;

        taggers.Remove(tagger.gameObject);
        tagger.SetTagger(false);

        taggers.Add(tagged.gameObject);
        tagged.SetTagger(true);
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
            stream.SendNext(taggers);
        }
        else if (stream.IsReading)
        {
            taggers = (List<GameObject>)stream.ReceiveNext();
        }
    }
}
