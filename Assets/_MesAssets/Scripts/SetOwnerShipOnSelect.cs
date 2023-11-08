using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class SetOwnerShipOnSelect : NetworkBehaviour
{
    // Variable r�seau qui indique si la hache est saisie ou non
    public NetworkVariable<bool> isNetworkGrabbed = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private XRBaseInteractable _interactable; // repr�sente le grab interactable de l'objet
    
    private void Start()
    {
        _interactable = GetComponent<XRBaseInteractable>(); // recup�re le componenet dans la variable
        // D�tecte l'�v�nement de la saisie et appelle la m�thode
        _interactable.selectEntered.AddListener(x => SetOwnerShip());
        // D�tecte quand le joueur l�che l'objet pour changer la valeur du bool�en
        _interactable.selectExited.AddListener(x => UnGrab());

    }

    // M�thode qui change le propri�taire de l'objet
    public void SetOwnerShip()
    {
        //Comme c'est le serveur qui doit changer le propri�taire je call le serverRpc
        //en lui envoyant le Id du joueur qui a saisi l'objet
        SetOwnershipServerRPC(NetworkManager.Singleton.LocalClientId);
    }

    // Commme seulement le serveur est autoris� � changer le propri�taire d'un objet
    // je dois utiliser un ServerRpc pour effectuer le changement
    // De plus comme n'importe quel joueur pour caller ce ServerRpc je met le RequireOwnership � false
    [ServerRpc(RequireOwnership = false)]
    void SetOwnershipServerRPC(ulong id)
    {
        NetworkObject.ChangeOwnership(id);
        // Change le bool�en pour indiquer qu'un joueur l'a dans les mains
        isNetworkGrabbed.Value = true;
    }

    // M�thode qui change le bool�en quand la hache est l�ch�e
    public void UnGrab()
    {
        UnGrabServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void UnGrabServerRpc()
    {
        isNetworkGrabbed.Value = false;
    }
}
