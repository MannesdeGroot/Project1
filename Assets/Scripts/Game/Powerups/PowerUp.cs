using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PowerUp : MonoBehaviour
{
    protected PlayerController player;
    public Sprite icon;
    public string powerUpName;

    public abstract void Use();

    public void OnTriggerEnter(Collider c)
    {
        PlayerController _player = c.GetComponent<PlayerController>();
        if (_player == null) return;

        if(_player.powerUp == null)
        {
            _player.AddPowerUp(this);
            player = _player;
            ActivateSelf(false);
        }
    }

    private void ActivateSelf(bool value)
    {
        GetComponent<Renderer>().enabled = value;
        GetComponent<Collider>().enabled = value;
    }
}
