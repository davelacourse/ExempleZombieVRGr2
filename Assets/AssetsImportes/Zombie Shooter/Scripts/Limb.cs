using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb : MonoBehaviour
{
    public void Hit(GameObject hitby)
    {
        Zombie zombieParent = GetComponentInParent<Zombie>();
        if (zombieParent)
            zombieParent.Death();

        //Destroy the bullet
        Destroy(hitby);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Weapon"))
            Hit(collision.gameObject);
    }
}
