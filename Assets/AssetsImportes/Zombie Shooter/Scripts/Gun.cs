using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Gun : NetworkBehaviour
{
    public float speed = 40;
    public GameObject bullet;
    public Transform barrel;
    public AudioSource audioSource;
    public AudioClip audioClip;

    public void Fire()
    {
        FireServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void FireServerRpc()
    {
        GameObject spawnedBullet = Instantiate(bullet, barrel.position, barrel.rotation);

        //Instancie la balle sur le réseau
        spawnedBullet.GetComponent<NetworkObject>().Spawn(true);

        spawnedBullet.GetComponent<Rigidbody>().velocity = speed * barrel.forward;
        audioSource.PlayOneShot(audioClip);
        Destroy(spawnedBullet, 2);
    }
}
