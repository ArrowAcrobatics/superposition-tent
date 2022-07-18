using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformEnforcer : MonoBehaviour
{
    public Transform targetTransform;
    public bool setPosition = true;
    public bool setRotation = true;

    /**
     * If this is set to true, the position of this game component will be achieved by
     * setting the position of the youngest ancestor with a TransformEnforcerAnchor component
     * or root if that isn't found. 
     * 
     * Intermediate tranforms are left untouched.
     */
    public bool propagateToParents = true;

    void FixedUpdate() {
        Transform anchor = targetTransform;
        if(propagateToParents) {
            while(anchor.GetComponent<TransformEnforcerAnchor>() == null && anchor.parent != null) {
                anchor = anchor.parent;
            }
        }

        // update targetTransform
        if(setPosition) {
            anchor.position = anchor.position - targetTransform.position + transform.position;
            //targetTransform.position = transform.position;
        }
        if(setRotation) {
            targetTransform.rotation = transform.rotation;
        }
    }
}
