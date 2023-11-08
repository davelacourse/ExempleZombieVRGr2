using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class NetworkChoisirCouleurAleatoire : NetworkBehaviour
{
    [SerializeField] private List<Renderer> _renderers = default;
    private NetworkVariable<Color> _networkColor = new NetworkVariable<Color>(Color.blue, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        // Permet de souscrire à la méthode SetRendererColor lors d'un changement de couleur
        // ce qui va permettre de synchroniser la valeur de la variable réseau pour cet avatar
        _networkColor.OnValueChanged += (x, y) => SetRendererColor(y);

        // Je détermine une couleur aléatoire seulement si je suis le propriétaire de l'avater
        if (IsOwner)
        {
            _networkColor.Value = Random.ColorHSV(0, 1, 1, 1);
        }

        SetRendererColor(_networkColor.Value);
    }

    public override void OnNetworkDespawn()
    {
        // Doit s'assurer d'arrêter la souscription lorsque le réseau arrête
        _networkColor.OnValueChanged -= (x, y) => SetRendererColor(y);
    }

    public void SetRendererColor(Color newColor)
    {
        foreach (var item in _renderers)
        {
            item.material.color = newColor;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateColorServerRPC(new ServerRpcParams());  // Appelle le ServerRpc en transmettant les infos du client qui a
                                                          // déclencher l'appel
        }
    }

    // méthode qui utilise RPC Client les crochets sont obligatoire avec cette syntaxe ainsi que 
    // la syntaxe pour le nom de ma méthode qui se termine par ClientRPC
    // Une méthode ClientRpc fonctionne sur le host qui peut appeler les méthodes sur les clients
    // Cependant cela ne fonctionne pas pour que les clients appelle la méthode sur le serveur.
    [ClientRpc]
    public void UpdateColorClientRPC(ClientRpcParams clientParameters)  // Recoit en paramètre les informations du client ciblé
    {
        // On doit vérifier que c'est le Owner car la variable réseau peut être mofifier seulement par le Owner
        if (IsOwner)
        {
            _networkColor.Value = Random.ColorHSV(0, 1, 1, 1);
        }
    }

    // méthode qui utilise RPC Serveur les crochets sont obligatoire avec cette syntaxe ainsi que 
    // la syntaxe pour le nom de ma méthode qui se termine par ServerRpc
    // Une méthode ServerRpc fonctionne sur le client qui peut appeler les méthodes sur le host/serveur
    [ServerRpc(RequireOwnership =false)] 
    public void UpdateColorServerRPC(ServerRpcParams parameters) 
    {
        Debug.Log("Le joueur qui a appuyé sur la touche est : " +  parameters.Receive.SenderClientId);  // On affiche le client ID du
                                                                                                        // client qui a peser sur la touche
        
        ClientRpcParams clientParameters = new ClientRpcParams();  // Creer une nouvelle variable pour contenir les informations clients
        clientParameters.Send.TargetClientIds =  new List<ulong>() { 0 };  //Définit la liste des clients que nous voulons cibler
                                                                           //dans notre cas ça sera seulement le premier { 0 } 0=
                                                                           //le clientId qu'on veut ciblé
        UpdateColorClientRPC(clientParameters);
    }
}
