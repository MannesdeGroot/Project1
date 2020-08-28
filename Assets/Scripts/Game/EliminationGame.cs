using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EliminationGame : MiniGame
{
    [SerializeField] protected bool oneLive;
    protected List<Player> eliminated = new List<Player>();

    protected override void StartRound()
    {
        base.StartRound();

        if (!oneLive)
        {
            foreach(Player player in players)
            {
                player.score++;
            }

            foreach(Player player in eliminated)
            {
                players.Add(player);
            }
        }
    }

    public void EliminatePlayer(Player player)
    {
        if (!players.Contains(player)) return;

        players.Remove(player);
        eliminated.Add(player);
    }
}
