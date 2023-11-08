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
        // Permet de souscrire � la m�thode SetRendererColor lors d'un changement de couleur
        // ce qui va permettre de synchroniser la valeur de la variable r�seau pour cet avatar
        _networkColor.OnValueChanged += (x, y) => SetRendererColor(y);

        // Je d�termine une couleur al�atoire seulement si je suis le propri�taire de l'avater
        if (IsOwner)
        {
            _networkColor.Value = Random.ColorHSV(0, 1, 1, 1);
        }

        SetRendererColor(_networkColor.Value);
    }

    public override void OnNetworkDespawn()
    {
        // Doit s'assurer d'arr�ter la souscription lorsque le r�seau arr�te
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
                                                          // d�clencher l'appel
        }
    }

    // m�thode qui utilise RPC Client les crochets sont obligatoire avec cette syntaxe ainsi que 
    // la syntaxe pour le nom de ma m�thode qui se termine par ClientRPC
    // Une m�thode ClientRpc fonctionne sur le host qui peut appeler les m�thodes sur les clients
    // Cependant cela ne fonctionne pas pour que les clients appelle la m�thode sur le serveur.
    [ClientRpc]
    public void UpdateColorClientRPC(ClientRpcParams clientParameters)  // Recoit en param�tre les informations du client cibl�
    {
        // On doit v�rifier que c'est le Owner car la variable r�seau peut �tre mofifier seulement par le Owner
        if (IsOwner)
        {
            _networkColor.Value = Random.ColorHSV(0, 1, 1, 1);
        }
    }

    // m�thode qui utilise RPC Serveur les crochets sont obligatoire avec cette syntaxe ainsi que 
    // la syntaxe pour le nom de ma m�thode qui se termine par ServerRpc
    // Une m�thode ServerRpc fonctionne sur le client qui peut appeler les m�thodes sur le host/serveur
    [ServerRpc(RequireOwnership =false)] 
    public void UpdateColorServerRPC(ServerRpcParams parameters) 
    {
        Debug.Log("Le joueur qui a appuy� sur la touche est : " +  parameters.Receive.SenderClientId);  // On affiche le client ID du
                                                                                                        // client qui a peser sur la touche
        
        ClientRpcParams clientParameters = new ClientRpcParams();  // Creer une nouvelle variable pour contenir les informations clients
        clientParameters.Send.TargetClientIds =  new List<ulong>() { 0 };  //D�finit la liste des clients que nous voulons cibler
                                                                           //dans notre cas �a sera seulement le premier { 0 } 0=
                                                                           //le clientId qu'on veut cibl�
        UpdateColorClientRPC(clientParameters);
    }
}
