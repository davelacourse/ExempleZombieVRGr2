using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class SetOwnerShipOnSelect : NetworkBehaviour
{
    // Variable réseau qui indique si la hache est saisie ou non
    public NetworkVariable<bool> isNetworkGrabbed = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private XRBaseInteractable _interactable; // représente le grab interactable de l'objet
    
    private void Start()
    {
        _interactable = GetComponent<XRBaseInteractable>(); // recupère le componenet dans la variable
        // Détecte l'évènement de la saisie et appelle la méthode
        _interactable.selectEntered.AddListener(x => SetOwnerShip());
        // Détecte quand le joueur lâche l'objet pour changer la valeur du booléen
        _interactable.selectExited.AddListener(x => UnGrab());

    }

    // Méthode qui change le propriétaire de l'objet
    public void SetOwnerShip()
    {
        //Comme c'est le serveur qui doit changer le propriétaire je call le serverRpc
        //en lui envoyant le Id du joueur qui a saisi l'objet
        SetOwnershipServerRPC(NetworkManager.Singleton.LocalClientId);
    }

    // Commme seulement le serveur est autorisé à changer le propriétaire d'un objet
    // je dois utiliser un ServerRpc pour effectuer le changement
    // De plus comme n'importe quel joueur pour caller ce ServerRpc je met le RequireOwnership à false
    [ServerRpc(RequireOwnership = false)]
    void SetOwnershipServerRPC(ulong id)
    {
        NetworkObject.ChangeOwnership(id);
        // Change le booléen pour indiquer qu'un joueur l'a dans les mains
        isNetworkGrabbed.Value = true;
    }

    // Méthode qui change le booléen quand la hache est lâchée
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
