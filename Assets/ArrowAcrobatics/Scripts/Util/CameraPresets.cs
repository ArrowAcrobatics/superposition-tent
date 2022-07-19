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
    // when slerping, we keep target transform until destination reached.
    private TransformData fromLocation = null;
    private TransformData toLocation = null;
    private float timePassedSinceLastSet = 0;
    public float CameraSpeed = 1;

    // TODO: ease curve

    /**
     * TODO: option to set parent
     * TODO: add 'immediate' option or generic speed option
     */
    public void setTargetTransform(TransformData targettransform) {
        toLocation = targettransform;
        fromLocation = null; // is set in update when null, might change between this call and next update depending on physics
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
}
