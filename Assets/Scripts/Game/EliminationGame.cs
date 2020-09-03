using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EliminationGame : MiniGame
{
    [SerializeField] protected bool oneLive;
    protected List<Player> eliminated = new List<Player>();

    public void EliminatePlayer(Player player)
    {
        if (!players.Contains(player)) return;

        players.Remove(player);
        eliminated.Add(player);

        //Vervang met daadwerkelijke spectator mode ofzo
        player.gameObject.SetActive(false);
    }

    protected override void LoadSettings()
    {
        base.LoadSettings();

        oneLive = GameSettings.oneLive;
    }
}
