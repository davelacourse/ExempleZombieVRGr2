using UnityEngine;
using Unity.Netcode;

public class NetworkSpawnManager : NetworkBehaviour
{
    [SerializeField] private Transform _player = default;
    [SerializeField] private Transform[] _spawnPositions;
    private NetworkVariable<int> _nbJoueursReseau = new NetworkVariable<int>(0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);

    private void Start()
    {
        // Souscris � une m�thode lorsque le client se connecte
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
    }

    // M�thode appeler lors de la connection d'un client on re�oit en param�tre l'ID du client
    private void Singleton_OnClientConnectedCallback(ulong clientID)
    {
        // Si le client qui viens de se connecter est le joueur local on place le joueur sur un Spawn Point
        if(clientID == NetworkManager.Singleton.LocalClientId)
        {
            SetPlayerPosition();
        }

        // Si je suis le host/serveur j'augmente la valeur pour le nombre de joueur
        // car tel que d�fini seulement le serveur ou le host peut modifier la variable r�seau
        if (IsServer)
        {
            _nbJoueursReseau.Value++;
        }

        // S�curit� si le nombre de joueur est plus grand que le nombre de SpawnPoint on revient au premier
        if(_nbJoueursReseau.Value == _spawnPositions.Length)
        {
            _nbJoueursReseau.Value = 0;
        }
    }

    public void SetPlayerPosition()
    {
        int index = _nbJoueursReseau.Value;
        _player.position = _spawnPositions[index].position;
        _player.rotation = _spawnPositions[index].rotation;
    }
}
