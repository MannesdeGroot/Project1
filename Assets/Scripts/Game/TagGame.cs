using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagGame : EliminationGame
{
    public int taggersAmount;
    private List<Player> taggers;

    protected override void StartRound()
    {
        base.StartRound();

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

    protected override void EndRound()
    {
        base.EndRound();

        foreach (Player player in taggers)
        {
            EliminatePlayer(player);
        }

        if (oneLive)
        {
            if (players.Count > 1)
                StartRound();
            else
            {
                if (state != GameState.FINISHED)
                {
                    print($"Winner is {players[0].userName}");
                    state = GameState.FINISHED;
                    EndGame();
                }
            }
        }
        else
        {
            StartRound();
        }
    }

    public void TagPlayer(Player tagger, Player tagged)
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
}
