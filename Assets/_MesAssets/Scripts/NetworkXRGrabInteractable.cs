using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkXRGrabInteractable : XRGrabInteractable
{
    // Variable bas� sur la classe qui change le propri�taire et le boo�en
    private SetOwnerShipOnSelect _selectOwner;

    private void Start()
    {
        // Pour l'objet saisi r�cup�re le script sur celui-ci
        _selectOwner = GetComponent<SetOwnerShipOnSelect>();
    }

    // Vient remplacer la m�thode d'origine du XRGrabdInteractable qui
    // indique qui peut saisir et manipuler l'objet
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        
        // Place le bool�en a vrai qui indique qu'on peut manipuler l'objet si le bool�en est faux
        // ou si le bool�en est vrai mais que nous sommes propri�taire de l'objet en ce moment
        // dans tous autres cas le bool�en est plac� � faux et on ne pourra manipuler l'objet
        bool isNetworkGrabbable = (!_selectOwner.isNetworkGrabbed.Value) || (_selectOwner.isNetworkGrabbed.Value && _selectOwner.IsOwner);
        
        // retourne la vrai le rayon pointe sur l'objet et la condition pr�c�dente est � vrai
        return base.IsSelectableBy(interactor) && isNetworkGrabbable;
    }
}
