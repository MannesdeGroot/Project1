using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGamePlayer : MonoBehaviour
{
    Player player;
    MeshRenderer render;
    public Color runner, tagger;

    void Start()
    {
        player = GetComponent<Player>();
        render = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        render.material.color = player.isTagger ? tagger : runner;
    }
}
