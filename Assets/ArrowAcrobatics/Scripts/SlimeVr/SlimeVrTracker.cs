using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * just a tag class for now.
 */
public class SlimeVrTracker : MonoBehaviour
{
    [Tooltip("The target transform will be driven to identical position/rotation in world space coordinates as this trackers transform.")]
    public Transform targetTransform = null;
    private Vector3 targetTransformNeutralPos;
    private Quaternion targetTransformNeutralRot;
    // also copy our transforms in awake (and on tracker recenter)...
    private Vector3 transformNeutralPos;
    private Quaternion transformNeutralRot;

    public bool setPosition = false;
    public bool setRotation = true;
    public bool invertRotation = false;


    /**
     * just a nullable wrapper for Vector3
     */
    [System.Serializable]
    public class LocalEulerAngles
    {
        public Vector3 angles;

        public LocalEulerAngles(float x, float y, float z) {
            angles = new Vector3(x, y, z);
        }
        public LocalEulerAngles(Vector3 v) {
            angles = v;
        }

        public static implicit operator LocalEulerAngles(Vector3 v) => new LocalEulerAngles(v);
        public static implicit operator Vector3(LocalEulerAngles l) => l.angles;
    }
    public LocalEulerAngles localEulerAngles = null;

    // if transform.parent has a tracker component, this is cached here
    private SlimeVrTracker parentTracker = null;

    // the client instance that initialized us.
    private SlimeVrClient hostClient;
    private bool resetCallbackEnabled = false;

    /**
     * if host == null, this method functions as 'deinitialize'
     */
    public void Initialize(SlimeVrClient host) {
        hostClient = host;
        EnableResetCallback(true);
    }

    #region Unity Callbacks
    void Awake() {
        GetParentTracker();
        GetTargetNeutral();
        GetTrackerNeutral();
    }

    void OnEnable() {
        EnableResetCallback(true);
    }

    void OnDisable() {
        EnableResetCallback(false);
    }

    void Update() {
        if(parentTracker != null) {
            localEulerAngles = transform.localRotation.eulerAngles;
        } else {
            localEulerAngles = null;
        }
    }

    void FixedUpdate() {
        // update targetTransform
        if(targetTransform == null) {
            return;
        }

        if(setPosition) {
            targetTransform.position = targetTransformNeutralPos + (-transformNeutralPos + transform.position);
        }
        if(setRotation) {
            Quaternion myRotation = invertRotation ? Quaternion.Inverse(transform.rotation) : transform.rotation;

            targetTransform.rotation = targetTransformNeutralRot * (myRotation * Quaternion.Inverse(transformNeutralRot));
        }
    }

    #endregion

    void EnableResetCallback(bool enable) {
        if(hostClient != null) {
            if(enable) {
                if(!resetCallbackEnabled) {
                    Debug.Log("enabling slime reset callback");
                    hostClient.slimeVrResetEvent += OnSlimeReset;
                    resetCallbackEnabled = true;
                }
            } else {
                if(resetCallbackEnabled) {
                    Debug.Log("disabling slime reset callback");
                    hostClient.slimeVrResetEvent -= OnSlimeReset;
                    resetCallbackEnabled = false;
                }
            }
        }
    }

    void OnSlimeReset(object sender, EventArgs e) {
        Debug.Log("tracker responded to slime reset");
        GetParentTracker();
        GetTrackerNeutral();
    }

    void GetParentTracker() {
        parentTracker = transform.parent.GetComponent<SlimeVrTracker>();
    }

    // sets neutral values for the target transform to the current pos/rot.
    void GetTargetNeutral() {
        if(targetTransform == null) {
            return;
        }

        targetTransformNeutralPos = new Vector3(targetTransform.position.x, targetTransform.position.y, targetTransform.position.z);
        targetTransformNeutralRot = new Quaternion(targetTransform.rotation.x, targetTransform.rotation.y, targetTransform.rotation.z, targetTransform.rotation.w);
    }

    // sets neutral values for our transform to the current pos/rot.
    void GetTrackerNeutral() {
        transformNeutralPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        transformNeutralRot = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
    }
}
