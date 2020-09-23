using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PowerupSpawn : MonoBehaviour, Photon.Pun.IPunObservable
{
    public float minRespawnTime;
    public float maxRespawnTime;
    public List<GameObject> powerups = new List<GameObject>();
    public float timeToSpawnNext;
    public PhotonView pv;
    GameObject spawnedObj;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            timeToSpawnNext = Random.Range(minRespawnTime, maxRespawnTime);

        }
    }
    void Update()
    {
        if (PhotonNetwork.IsMasterClient && spawnedObj == null)
        {
            timeToSpawnNext -= Time.deltaTime;
            if(timeToSpawnNext < 0 )
            {
                //pv.RPC("SpawnPowerup", RpcTarget.All, Random.Range(0, powerups.Count));
                SpawnPowerup(Random.Range(0, powerups.Count));
                timeToSpawnNext = Random.Range(minRespawnTime, maxRespawnTime);

            }
        }
    }

    
    public void SpawnPowerup(int powerupIndex)
    {
        spawnedObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", powerups[powerupIndex].name), transform.position, Quaternion.identity);


    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(isTagger);
            
        }
        else if (stream.IsReading)
        {
            //isTagger = (bool)stream.ReceiveNext();
            
        }
    }
}
