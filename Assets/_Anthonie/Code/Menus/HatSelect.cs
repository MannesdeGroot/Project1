using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatSelect : MonoBehaviour
{
    public List<GameObject> hats;
    public int currentHatSelected;
    public Transform hatPos;

    public void SetHat()
    {
        for (int i = 0; i < hatPos.childCount; i++)
        {
            Destroy(hatPos.GetChild(i).gameObject);
        }
        if(currentHatSelected > 0)
        {
            Instantiate(hats[currentHatSelected], hatPos);
        }
    }

    public void NextHat()
    {
        currentHatSelected++;
        if(currentHatSelected < hats.Count)
        {
            SetHat();
        }
        else
        {
            currentHatSelected = 0;
            SetHat();
        }
    }

    public void PreviousHat()
    {
        currentHatSelected--;
        if (currentHatSelected > -1)
        {
            SetHat();
        }
        else
        {
            currentHatSelected = hats.Count - 1;
            SetHat();
        }
    }
}
