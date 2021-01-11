using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameControllerScript : MonoBehaviour
{
    bool waveStarted;
    float timeBeforeNextSpawn;

    [SerializeField]
    int maxZombie;
    public int getMaxZombie
    {
        get { return maxZombie; }
    }

    List<GameObject> SpawnedZombie;
    public int getSpawnedZombieCount
    {
        get { return SpawnedZombie.Count; }
    }

    [SerializeField]
    Transform[] Spawners;
    [SerializeField]
    GameObject Zombie;
    [SerializeField]
    public Camera GameCam;

    // Start is called before the first frame update
    void Start()
    {
        SpawnedZombie = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(SpawnedZombie.Count == 0 && !waveStarted)
        {
            waveStarted = true;
            try
            {
                spawnZombie(ThreadingUtility.QuitToken);
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
        }
        else if(SpawnedZombie.Count == 0 && waveStarted)
        {
            waveStarted = false;
            maxZombie = (int)Math.Floor(1.5f * maxZombie);
        }
    }

    async void spawnZombie(CancellationToken token)
    {
        for (int i = 0; i < maxZombie; i++)
        {
            token.ThrowIfCancellationRequested();
            while (Time.time < timeBeforeNextSpawn)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(TimeSpan.FromSeconds(.1));
            }
            int randomInt = Random.Range(0, Spawners.Length - 1);
            SpawnedZombie.Add(Instantiate(Zombie, Spawners[randomInt].position, new Quaternion()));
            timeBeforeNextSpawn = Time.time + 1;
        }
    }

    public void removeDeadZombieFromList(GameObject deadzombie)
    {
        SpawnedZombie.Remove(deadzombie);
    }
}
