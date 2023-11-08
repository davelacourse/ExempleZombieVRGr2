using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkXRGrabInteractable : XRGrabInteractable
{
    // Variable basé sur la classe qui change le propriétaire et le booéen
    private SetOwnerShipOnSelect _selectOwner;

    private void Start()
    {
        // Pour l'objet saisi récupère le script sur celui-ci
        _selectOwner = GetComponent<SetOwnerShipOnSelect>();
    }

    // Vient remplacer la méthode d'origine du XRGrabdInteractable qui
    // indique qui peut saisir et manipuler l'objet
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        
        // Place le booléen a vrai qui indique qu'on peut manipuler l'objet si le booléen est faux
        // ou si le booléen est vrai mais que nous sommes propriétaire de l'objet en ce moment
        // dans tous autres cas le booléen est placé à faux et on ne pourra manipuler l'objet
        bool isNetworkGrabbable = (!_selectOwner.isNetworkGrabbed.Value) || (_selectOwner.isNetworkGrabbed.Value && _selectOwner.IsOwner);
        
        // retourne la vrai le rayon pointe sur l'objet et la condition précédente est à vrai
        return base.IsSelectableBy(interactor) && isNetworkGrabbable;
    }
}
