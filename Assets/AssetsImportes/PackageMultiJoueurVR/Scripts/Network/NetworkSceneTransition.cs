using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;


public class NetworkSceneTransition : MonoBehaviour
{
    public static NetworkSceneTransition Instance;
    
    private bool _isLoading = false; //Bolléean qui détermine si la scène est chargée

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // S'assurer que le Serveur est en marche
        NetworkManager.Singleton.OnServerStarted += ServerStarted;
    }

    private void ServerStarted()
    {
        // Quand le serveur est démarrer on vérifie que la scène est chargée
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
    }

    private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        // Quand la scène est chargée on change le booléen
        _isLoading = false;
    }

    public void ChargerScenePourTous(string sceneName)
    {
        if(!_isLoading)
        {
            _isLoading = true;
            // en utilisant le network manager le changement de scène s'effectue pour tous les joueurs
            // branchés à notre partie réseau et aussi pour tous les joueurs futurs.
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}


