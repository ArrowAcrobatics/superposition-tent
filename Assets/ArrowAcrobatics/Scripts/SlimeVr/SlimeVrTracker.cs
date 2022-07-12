using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * just a tag class for now.
 */
public class SlimeVrTracker : MonoBehaviour
{
    [System.Serializable]
    public class TripointAngle {
        // this tracker is the middle,
        // left and right are either children or the parent of this object.
        public SlimeVrTracker left;
        public SlimeVrTracker right;
        public float angle;
    }

    public List<TripointAngle> angles;

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

    void OnEnable() {
        EnableResetCallback(true);
    }

    void OnDisable() {
        EnableResetCallback(false);
    }

    void OnSlimeReset(object sender, EventArgs e) {
        Debug.Log("tracker responded to slime reset");
        FindTripoints();
    }

    void FindTripoints() {
        List<SlimeVrTracker> nodes = new List<SlimeVrTracker>();

        SlimeVrTracker parentTracker = transform.parent.GetComponent<SlimeVrTracker>();
        if(parentTracker != null) {
            nodes.Add(parentTracker);
        }

        foreach(Transform child in transform) {
            SlimeVrTracker tracker = child.GetComponent<SlimeVrTracker>();
            if(tracker != null) {
                nodes.Add(tracker);
            }
        }

        // go over all ordered pairs in nodes.
        angles = new List<TripointAngle>();
        for (int i = 0; i < nodes.Count; i++) {
            for (int j = 0; j < i; j++) {
                angles.Add(new TripointAngle {
                    left = nodes[i],
                    right = nodes[j],
                    angle = 0
                });
            }
        }
    }
}
