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

    public bool gamePaused { get; private set; }
    public int round { get; private set; }
    public List<GameObject> SpawnedZombie { get; private set; }
    public int MaxZombie 
    { 
        get 
        { 
            return maxZombie; 
        } 
        private set 
        { 
            maxZombie = value; 
        } 
    }

    public int playerPoint { get; private set; }

    [SerializeField]
    Transform[] Spawners;
    [SerializeField]
    GameObject Zombie;
    [SerializeField]
    public Camera GameCam;
    [SerializeField]
    int maxZombie;

    // Start is called before the first frame update
    void Start()
    {
        SpawnedZombie = new List<GameObject>();
        round = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(SpawnedZombie.Count == 0 && !waveStarted)
        {
            waveStarted = true;
            spawnZombie(ThreadingUtility.QuitToken);
        }
        else if(SpawnedZombie.Count == 0 && waveStarted)
        {
            waveStarted = false;
            maxZombie = (int)Math.Floor(1.5f * maxZombie);
            round++;
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

    public int getRound()
    {
        return round;
    }

    public void AddPointToPlayer(old_PlayerScript player, int pointToAdd)
    {
        playerPoint += pointToAdd;
    }

    public void PauseControl()
    {
        Time.timeScale = gamePaused ? 1 : 0;
        gamePaused = !gamePaused;

        foreach(old_PlayerScript pScript in FindObjectsOfType(typeof(old_PlayerScript)))
        {
            pScript.ShowPauseMenu();
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
