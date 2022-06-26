using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.XR.Interaction.Toolkit;

/**
 * Usage: 
 *  - add locomotion system to XR rig.
 *  - add input action
 * 
 * put left hand controller on the floor. Then press button. This sets the floor level.
 */
[RequireComponent(typeof(TeleportationProvider))]
public class TeleportationManager : MonoBehaviour
{
    [SerializeField] 
    private TeleportationProvider _teleportationProvider = null;

    [SerializeField] 
    private InputActionReference _recenterAction = null;

    [SerializeField]
    private Transform _cameraTransform = null;

    [SerializeField]
    private Transform _leftHandTransform = null;

    [SerializeField]
    private Transform _rightHandTransform = null;

    [SerializeField]
    private Transform _rigTransform = null;

    [SerializeField]
    private AudioClip _teleportSfx;

    private AudioSource _audioSource = null;

    //public MatchOrientation matchOrientation = MatchOrientation.None;
    //public bool invertOrientation = false;


    void OnEnable() {
        if(_teleportationProvider == null 
            || _recenterAction == null 
            || _cameraTransform == null
            || _leftHandTransform == null
            || _rightHandTransform == null
            || _rigTransform == null) {
            Debug.Log("TeleportationManager not set up correctly.");
            _teleportationProvider = GetComponent<TeleportationProvider>();
        }

        if(_recenterAction) {
            _recenterAction.action.performed += OnTeleportButtonClick;
        }

        _audioSource = GetComponent<AudioSource>();
    }

    void OnDisable() {
        if(_recenterAction) {
            _recenterAction.action.performed -= OnTeleportButtonClick;
        }
    }


    void OnTeleportButtonClick(InputAction.CallbackContext ctx) {
        Debug.Log("teleport clicked");

        TeleportRequest req = new TeleportRequest();
        req.destinationPosition = new Vector3(0, _rigTransform.position.y-_rightHandTransform.position.y, 0);
        req.destinationRotation = Quaternion.identity;
        req.matchOrientation = MatchOrientation.TargetUpAndForward;

        _teleportationProvider.QueueTeleportRequest(req);

        if(_audioSource != null && _teleportSfx != null) {
            _audioSource.PlayOneShot(_teleportSfx, _audioSource.volume);
        }
    }
}
