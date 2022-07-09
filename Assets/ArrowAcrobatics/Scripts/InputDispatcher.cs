using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDispatcher : MonoBehaviour
{    
    [SerializeField] InputActionReference leftTrigger;
    [SerializeField] InputActionReference leftGrip;
    [SerializeField] InputActionReference leftThumbNear;
    [SerializeField] InputActionReference leftThumbFar;

    [SerializeField] InputActionReference rightTrigger;
    [SerializeField] InputActionReference rightGrip;
    [SerializeField] InputActionReference rightThumbNear;
    [SerializeField] InputActionReference rightThumbFar;

    void OnEnable() {
        if(leftTrigger != null) { leftTrigger.action.performed += logAction; }
        if(leftGrip != null) { leftGrip.action.performed += logAction; }
        if(leftThumbNear != null) { leftThumbNear.action.performed += logAction; }
        if(leftThumbFar != null) { leftThumbFar.action.performed += logAction; }

        if(rightTrigger != null) { rightTrigger.action.performed += logAction; }
        if(rightGrip != null) { rightGrip.action.performed += logAction; }
        if(rightThumbNear != null) { rightThumbNear.action.performed += logAction; }
        if(rightThumbFar != null) { rightThumbFar.action.performed += logAction; }
    }

    void OnDisable() {
        if(leftTrigger != null) { leftTrigger.action.performed -= logAction; }
        if(leftGrip != null) { leftGrip.action.performed -= logAction; }
        if(leftThumbNear != null) { leftThumbNear.action.performed -= logAction; }
        if(leftThumbFar != null) { leftThumbFar.action.performed -= logAction; }

        if(rightTrigger != null) { rightTrigger.action.performed -= logAction; }
        if(rightGrip != null) { rightGrip.action.performed -= logAction; }
        if(rightThumbNear != null) { rightThumbNear.action.performed -= logAction; }
        if(rightThumbFar != null) { rightThumbFar.action.performed -= logAction; }
    }

    void logAction(InputAction.CallbackContext ctx) {
        Debug.Log("action handled" + ctx.ToString());
    }
}
