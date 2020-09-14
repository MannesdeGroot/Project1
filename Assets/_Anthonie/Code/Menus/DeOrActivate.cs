using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeOrActivate : MonoBehaviour
{
    public GameObject objectToActivate;
    public GameObject objectToDeactivate;
    
    public void Activate()
    {
        objectToActivate.SetActive(true);
    }

    public void Deactivate()
    {
        objectToDeactivate.SetActive(false);
    }

    public void DeAndActivate()
    {
        Activate();
        Deactivate();
    }
}
