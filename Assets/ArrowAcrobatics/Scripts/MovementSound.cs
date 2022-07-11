using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Measures angles/velocities/relavite position and turns it into sound.
 * 
 * TODO: rename to SkeletonEnforcer or something
 */
public class MovementSound : MonoBehaviour
{
    [Tooltip("The parent and child will be searched for in this object")]
    public GameObject HostObject;
    public string parentName = "";
    public string childName = "";
    
    public GameObject soundParent;
    public GameObject soundChild;

    bool restPositionAqcuired { get { return restPositionParent != null && restPositionChild != null; } }
    private GameObject restPositionParent;
    private GameObject restPositionChild;
    
    private GameObject globalFolderObject;

    static string FolderObjectName = "Folder_MovementSound";

    public void Awake() {
        globalFolderObject = GameObject.Find("/" + FolderObjectName);

        if(globalFolderObject == null) {
            globalFolderObject = new GameObject(FolderObjectName);
        }
    }

    public void Update() {
        FindTargetObjects();
        EnforceParentRelationship();
        if(!restPositionAqcuired) {
            ResetRestPosition();
        }
    }

    void FindTargetObjects() {
        Transform t;
        if(soundParent == null) {
            t = HostObject.transform.Find(parentName);
            if(t != null) {
                soundParent = t.gameObject;
            }
        }

        if(soundChild == null) {
            t = HostObject.transform.Find(childName);
            if(t != null) {
                soundChild = t.gameObject;
            }
        }
    }

    void EnforceParentRelationship() {
        if (soundChild != null && soundParent != null && soundChild.transform.parent != soundParent) {
            soundChild.transform.SetParent(soundParent.transform, true);
        }
    }

    [ContextMenu("resetPosition")]
    public void ResetRestPosition() {
        if (restPositionParent != null) {
            Destroy(restPositionParent);
        }
        if(restPositionChild != null) {
            Destroy(restPositionChild);
        }

        if(soundParent!= null) {
            restPositionParent = new GameObject("rest_" + soundParent.name);
            restPositionParent.transform.position = soundParent.transform.position;
            restPositionParent.transform.rotation = soundParent.transform.rotation;
            restPositionParent.transform.parent = globalFolderObject.transform;
        }

        if(soundChild != null) {
            restPositionChild = new GameObject("rest_" + soundChild.name);
            restPositionChild.transform.position = soundChild.transform.position;
            restPositionChild.transform.rotation = soundChild.transform.rotation;
            restPositionChild.transform.parent = globalFolderObject.transform;
        }
    }
}
