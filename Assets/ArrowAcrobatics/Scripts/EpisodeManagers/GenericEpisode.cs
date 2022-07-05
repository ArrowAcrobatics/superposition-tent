using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Most episodes can simply set a bunch of game objects active/inactive
 * and perhaps need a tiny bit of custom logic after that.
 */
public class GenericEpisode : MonoBehaviour
{
    public GameObject[] _objects;

    public void launch() {
        Debug.Log("launching episode " + gameObject.name);

        foreach(GameObject obj in _objects) {
            if(obj != null) {
                obj.SetActive(true);
            }
        }

        OnLaunch();
    }
    public void stop() {
        Debug.Log("stopping episode " + gameObject.name);
        foreach(GameObject obj in _objects) {
            if(obj != null) {
                obj.SetActive(false);
            }
        }

        OnStop();
    }


    // extend these in subclasses
    public virtual void OnLaunch() {

    }

    public virtual void OnStop() {

    }

}
