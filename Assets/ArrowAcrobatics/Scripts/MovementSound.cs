using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Measures angles/velocities/relavite position and turns it into sound.
 */
public class MovementSound : MonoBehaviour
{
    public GameObject soundParent;
    public GameObject soundChild;

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

    public void OnEnable() {
        resetRestPosition();
    }

    [ContextMenu("resetPosition")]
    public void resetRestPosition() {
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
