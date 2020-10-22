using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSpawn : MonoBehaviour
{
    public AudioSource audioSource;
    public float randomPitch;
    public float destroyAfterTime;
    void Start()
    {
        audioSource.pitch += Random.Range(-randomPitch, randomPitch);
        audioSource.Play();
    }

    void Update()
    {
        destroyAfterTime -= Time.deltaTime;
        if(destroyAfterTime < 0)
        {
            Destroy(gameObject);
        }
    }
}
