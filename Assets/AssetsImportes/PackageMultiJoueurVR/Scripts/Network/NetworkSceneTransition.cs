using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;


public class NetworkSceneTransition : MonoBehaviour
{
    public static NetworkSceneTransition Instance;
    
    private bool _isLoading = false; //Boll�ean qui d�termine si la sc�ne est charg�e

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
        // Quand le serveur est d�marrer on v�rifie que la sc�ne est charg�e
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
    }

    private void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        // Quand la sc�ne est charg�e on change le bool�en
        _isLoading = false;
    }

    public void ChargerScenePourTous(string sceneName)
    {
        if(!_isLoading)
        {
            _isLoading = true;
            // en utilisant le network manager le changement de sc�ne s'effectue pour tous les joueurs
            // branch�s � notre partie r�seau et aussi pour tous les joueurs futurs.
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}


