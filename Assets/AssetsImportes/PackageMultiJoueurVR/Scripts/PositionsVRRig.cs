using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionsVRRig : MonoBehaviour
{   
    public static PositionsVRRig Instance;
    
    // Attributs qui servent à contenir la position de l'équipement de VR
    [SerializeField] private Transform _root = default;  // XR Origin
    public Transform Root => _root;

    [SerializeField] private Transform _head = default;  // Main Camera
    public Transform Head => _head;

    [SerializeField] private Transform _leftHand = default;  // LeftHand
    public Transform LeftHand => _leftHand;

    [SerializeField] private Transform _rightHand = default;  // RightHand
    public Transform RightHand => _rightHand;

    private void Awake()
    {
        Instance = this;
    }
}
