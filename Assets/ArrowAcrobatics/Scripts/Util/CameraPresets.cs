using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Makes this gameobject move towards the given gameobject (in a certain way)
 */
public class CameraPresets : MonoBehaviour
{
    public List<Transform> presets = new List<Transform>();
    public int currentCameraAngle = 0;

    // when slerping, we keep target transform until destination reached.
    private Transform target = null;

    [ContextMenu("set target")]
    void setTarget() {
        setTarget(currentCameraAngle);
    }

    [ContextMenu("next camera angle")]
    void nextCameraAngle() {
        setTarget(currentCameraAngle+1);
    }

    [ContextMenu("prev camera angle")]
    void prevtCameraAngle() {
        setTarget(currentCameraAngle-1);
    }

    void setTarget(int nextCameraAngle) {
        int index = getIndex(nextCameraAngle);
        Transform t = presets[index];
        target = t;
        currentCameraAngle = index;

        if(t == null) {
            return;
        }

#if UNITY_EDITOR
        Debug.Log("in editor mode, setting target to null and relocating immediately");
        transform.position = t.position;
        transform.rotation = t.rotation;
#else
        Debug.Log("in game position adjustments not yet implemented");
#endif


    }

    private int getIndex(int index) {
        if (index < 0) {
            return 0;
        }

        if(index >= presets.Count) {
            return presets.Count -1;
        }

        return index;
    }
}
