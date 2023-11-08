using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;  // ajoute la librairie réseau

public class Spawner : NetworkBehaviour  // change la classe hérité
{
    public float spawnTime = 1;
    public GameObject spawnGameObject;
    public Transform[] spawnPoints;
    private float timer;

    // Update is called once per frame
    void Update()
    {
        if(timer > spawnTime)
        {
            // Si je ne suis pas le serveur je resort du update car seulement le serveur
            // instancie les zombies et gère le timer
            if(!IsServer)
            {
                return;
            }
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject spawnedZombie = Instantiate(spawnGameObject, randomPoint.position, randomPoint.rotation);
            
            // Instancie le gameobject sur le réseau
            spawnedZombie.GetComponent<NetworkObject>().Spawn(true);
            
            timer = 0;
        }
        timer += Time.deltaTime;
    }
}


