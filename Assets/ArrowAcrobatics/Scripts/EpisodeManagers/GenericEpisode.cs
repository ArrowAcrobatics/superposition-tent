using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Most episodes can simply set a bunch of game objects active/inactive
 * and perhaps need a tiny bit of custom logic after that.
 */
public class GenericEpisode : MonoBehaviour
{
    public Transform _cameraPreset = null;
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

    /**
     * User requests a next state. (E.g. powerpoint slide)
     * 
     * return: 
     *  - true: this episode handled the event. Prevent going to next episode.
     *  - false: this episode didn't handle the event. accept going to next episode.
     */
    public virtual bool next() {
        Debug.Log("generic next");
        return false;
    }

    /**
     * User requests a previous state. (E.g. powerpoint slide)
     * 
     * return: 
     *  - true: this episode handled the event. Prevent going to next episode.
     *  - false: this episode didn't handle the event. accept going to next episode.
     */
    public virtual bool prev() {
        Debug.Log("generic prev");
        return false;
    }

}
