using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;

public class RelayManager : MonoBehaviour
{
    private UnityTransport _transport;
    // Création d'un Singleton accessible de partout
    public static RelayManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _transport = GetComponent<UnityTransport>();
    }

    // Méthode qui sera appeler pour définir les paramètres de connexion au Relay
    // maxPLayers représente le nombre maximum de joueurs qui se connecte
    // en même temps à notre partie
    public async Task<string> CreateRelayGame(int maxPlayer)
    {
        // Permet d'avoir une allocation sur le relais avec le nombre de joueurs reçu
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayer);
        // Alloue un joinCode au joueur pour joindre le Relay
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log ("The Join Code is : " + joinCode);
        // Configure toute les data nécessaire pour la connexion du Host au Relay
        _transport.SetHostRelayData(allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key,
            allocation.ConnectionData);
        //Démarrer le Host
        NetworkManager.Singleton.StartHost();

        return joinCode;
    }

    // Méthode qui prendre de faire la connexion avec le Relay
    // Elle reçoit le joinCode établit dans la méthode précédente
    public async void JoinRelayGame(string joinCode)
    {
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        // Configure toute les data nécessaire pour la connexion du Client au Relay
        _transport.SetClientRelayData(allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key,
            allocation.ConnectionData,
            allocation.HostConnectionData);
        // Démarrer le Client
        NetworkManager.Singleton.StartClient();
    }
}

