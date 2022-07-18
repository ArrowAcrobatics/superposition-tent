using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformEnforcer : MonoBehaviour
{
    public Transform targetTransform;
    public bool setPosition = true;
    public bool setRotation = true;

    public bool propagateToParents = true;

    void FixedUpdate() {
        // update targetTransform
        if(setPosition) {
            targetTransform.position = transform.position;
        }
        if(setRotation) {
            targetTransform.rotation = transform.rotation;
        }
    }
}
