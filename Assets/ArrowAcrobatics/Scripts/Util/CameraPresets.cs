using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ArrowAcrobatics.TransformExtensions;

/**
 * Makes this gameobject move towards the given gameobject (in a certain way)
 * A null preset will halt the currently running slerp action and leave the camera in place.
 */
public class CameraPresets : MonoBehaviour
{
    public List<Transform> presets = new List<Transform>();
    public int currentCameraAngle = 0;

    // when slerping, we keep target transform until destination reached.
    private TransformData fromLocation = null;
    private TransformData toLocation = null;
    private float timePassedSinceLastSet = 0;
    public float CameraSpeed = 1;

    // TODO: ease curve

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

        toLocation = t.GlobalData();
        fromLocation = null; // is set in update when null, might change between this call and next update depending on physics
        
        currentCameraAngle = index;

        if(toLocation == null) {
            return;
        }

#if UNITY_EDITOR
        transform.Set(toLocation);
        toLocation = null;
#else
        Debug.Log("in game position adjustments not yet implemented");
#endif
    }

    void Update() {
        if(toLocation != null) {
            if(fromLocation == null) {
                fromLocation = transform.GlobalData();
                timePassedSinceLastSet = 0;
            }

            float nextTime = Mathf.Clamp01(timePassedSinceLastSet + CameraSpeed * Time.deltaTime);
            
            if(nextTime >= 1.0f) {
                transform.Set(toLocation);
                timePassedSinceLastSet = 0;
                fromLocation = null;
                toLocation = null;
                Debug.Log("Cam preset reached");
            } else {
                transform.Set(TransformData.Slerp(fromLocation, toLocation, timePassedSinceLastSet));
                timePassedSinceLastSet = nextTime;
            }
            
        }
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
