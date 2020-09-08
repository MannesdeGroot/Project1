using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagGame : EliminationGame
{
    public int taggersAmount;
    private List<Player> taggers;

    public override void StartGame()
    {
        base.StartGame();

        foreach(Player player in players)
        {
            player.isTagger = false;
            player.SetTagger(false);
        }

        taggers = new List<Player>();

        for (int i = 0; i < taggersAmount; i++)
        {
            int random = Random.Range(0, players.Count);

            Player player = players[random];
            if (!taggers.Contains(player))
            {
                taggers.Add(player);
                player.SetTagger(true);
            }
            else
            {
                i--;
            }
        }
    }

    public void TagPlayer(Player tagger, Player tagged, float tagForceMultiplier)
    {
        if (tagged.isTagger) return;

        taggers.Remove(tagger);
        tagger.SetTagger(false);

        taggers.Add(tagged);
        tagged.SetTagger(true);

        Vector3 force = tagger.transform.forward * tagForceMultiplier;
        tagged.GetComponent<Rigidbody>().AddForce(force);
    }

    protected override void LoadSettings()
    {
        base.LoadSettings();

        taggersAmount = GameSettings.amountOfTaggers;
    }
}
