using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings
{
    //Universal Settings
    public static int maxPlayers = 10;
    public static int minPlayers = 2;
    public static bool timed = true;
    public static float matchTime = 60;
    public static bool limitedRounds;
    public static int maxRounds = 5;

    //Settings for elimination gamemodes
    public static bool oneLive = true;

    //Settings for Tag
    public static int amountOfTaggers = 1;
}
