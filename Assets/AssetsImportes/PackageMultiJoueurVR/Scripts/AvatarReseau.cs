using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AvatarReseau : NetworkBehaviour
{
    // Attributs qui servent à contenir les positions de mon avatar
    [SerializeField] private Transform _rootAvatar = default;  // XR Origin
    public Transform RootAvatar => _rootAvatar;

    [SerializeField] private Transform _headAvatar = default;  // Main Camera
    [SerializeField] private Transform _leftHandAvatar = default;  // LeftHand
    [SerializeField] private Transform _rightHandAvatar = default;  // RightHand

    private void Update()
    {
        if (IsOwner)
        {
            _rootAvatar.position = PositionsVRRig.Instance.Root.position;
            _rootAvatar.rotation = PositionsVRRig.Instance.Root.rotation;

            _headAvatar.position = PositionsVRRig.Instance.Head.position;
            _headAvatar.rotation = PositionsVRRig.Instance.Head.rotation;

            _leftHandAvatar.position = PositionsVRRig.Instance.LeftHand.position;
            _leftHandAvatar.rotation = PositionsVRRig.Instance.LeftHand.rotation;

            _rightHandAvatar.position = PositionsVRRig.Instance.RightHand.position;
            _rightHandAvatar.rotation = PositionsVRRig.Instance.RightHand.rotation;
        }
    }
}
