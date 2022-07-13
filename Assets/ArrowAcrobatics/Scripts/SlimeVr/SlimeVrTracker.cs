using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * just a tag class for now.
 */
public class SlimeVrTracker : MonoBehaviour
{
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
    }

    void GetParentTracker() {
        parentTracker = transform.parent.GetComponent<SlimeVrTracker>();
    }
}
