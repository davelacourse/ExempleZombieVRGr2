using UnityEngine;
using Unity.Netcode;  // Insertion librairie réseau

public class Limb : MonoBehaviour
{
    // Méthode appeler seulement par le serveur quand une balle frappe un zombie
    public void Hit(GameObject hitby)
    {
        ZombieNetwork zombieParent = GetComponentInParent<ZombieNetwork>();
        
        if (zombieParent)
            // On devra adapter cette méthode car bien qu'elle soit appelé par le serveur
            // On veux qu'elle soit appeler sur tous les clients
            zombieParent.Death();  

        //Destroy the bullet
        Destroy(hitby);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Si nous ne sommes pas le serveur on ne gère pas la collision des balles
        // avec les zombies donc on sort de la méthode
        if (!NetworkManager.Singleton.IsServer) 
            return;
        
        if (collision.gameObject.CompareTag("Weapon"))
            Hit(collision.gameObject);
    }
}


