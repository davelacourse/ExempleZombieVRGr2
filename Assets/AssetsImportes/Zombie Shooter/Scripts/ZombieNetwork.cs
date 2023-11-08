using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;  // Ajout librairie réseau

public class ZombieNetwork : NetworkBehaviour  //change l'héritage pour le behavior réseau
{
    public float minSpeed = 1f;
    public float maxSpeed = 4;
    public AudioClip deathAudio;
    public Transform target;
    private NavMeshAgent agent;
    private Rigidbody[] rbs;


    private AvatarReseau[] _players;



    // Start is called before the first frame update
    void Start()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        
        // En single player le zombie se dirige vers le joueur il faut l'adapter en multijoueur
        // comme c'est le serveur qui gère ceux-ci seulement le serveur va modifier leurs comportements
        if(IsServer)
        {
            // On récupère dans un tableau tous les joueurs présent
            _players = FindObjectsOfType<AvatarReseau>();
            // Chaque zombie choisi un joueur au hasard et devient son target
            target = _players[Random.Range(0, _players.Length)].RootAvatar;
        }
        else
        {
            // Si nous ne sommes pas le serveur on déactive le navmesh du zombie
            target = null;
            agent.enabled = false;
        }
        

        DisactivateRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            // Trouve le joueur le plus près du zombie et ce joueur devient le target
            float minDistance = float.MaxValue;
            foreach(var p in _players)
            {
                float distance = Vector3.Distance(p.transform.position,transform.position);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    target = p.RootAvatar;
                }
            }

            agent.SetDestination(target.position);

            
            // Vérifie si un des joueurs est à moins de 1m d'un zoombie la partie se termine
            foreach (var player in _players)
            {
                if (Vector3.Distance(player.transform.position, transform.position) < 1f)
                {
                    NetworkSceneTransition.Instance.ChargerScenePourTous("Zombie");
                }
            }

        }
    }

    // Cette méthode est appeler par le serveur comme on veut que tous les
    // client soit aussi updater j'appelle la clientRPC et mets à jour l'info 
    // sur tous les clients
    public void Death()
    {
        DeathClientRPC();
        // Le destroy se propage automatiquement à tous les clients
        Destroy(gameObject, 10); 
    }

    
    // Méthode client pour signifier la mort du zombie sur tous les clients
    [ClientRpc]
    public void DeathClientRPC()
    {
        ActivateRagdoll();
        agent.enabled = false;
        GetComponent<Animator>().enabled = false;
        AudioSource audioS = GetComponent<AudioSource>();
        audioS.loop = false;
        audioS.PlayOneShot(deathAudio);

        
        Destroy(this);
    }

    void ActivateRagdoll()
    {
        foreach (var item in rbs)
        {
            item.isKinematic = false;
        }
    }

    void DisactivateRagdoll()
    {
        foreach (var item in rbs)
        {
            item.isKinematic = true;
        }
    }
}
