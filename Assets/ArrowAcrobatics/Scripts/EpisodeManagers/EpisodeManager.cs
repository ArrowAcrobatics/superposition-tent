using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * Episode manager makes the transitions from one episode to the next.
 * 
 * A registered episode launcher is enabled when lauching an episode.
 * This is a gameobject in the scene responsible for loading, unloading
 * enabling etc of relevant game objects.
 * 
 * Action map:
 * Activate: wijsvinger (trigger)
 * Select: middelvinger (grip)
 */
public class EpisodeManager : MonoBehaviour
{
    private int _currentEpisodeIndex = 0;
    private GenericEpisode _currentEpisode = null;

    // we are storing game objects to avoid running into the old serializer problem hassles.
    public GameObject _defaultEpisode = null;
    public GameObject[] _episodeLaunchers;

    // ----------------------------------------

    [SerializeField] InputActionReference prevAction;
    [SerializeField] InputActionReference nextAction;

    // ----------------------------------------

    public bool wrapAround = false;
    public enum Loopmode
    {
        LoopSingle,
        LoopAll,
        LoopNone,
    }
    public Loopmode loopMode;
    
    public int debugEpisode = 0;

    [ContextMenu("Launch debug episode")]
    void LaunchDebugEpisode() {
        Debug.Log("Perform operation");
        launch(debugEpisode);
    }

    [ContextMenu("Launch prev")]
    public void prev() {
        int followingIndex = _currentEpisodeIndex-1;

        switch(loopMode) {
            case Loopmode.LoopNone: {
                launch(followingIndex);
                break;
            }
            case Loopmode.LoopAll: {
                launch(followingIndex >= 0 ? followingIndex : (_episodeLaunchers.Length - 1));
                break;
            }
            case Loopmode.LoopSingle: {
                launch(_currentEpisodeIndex);
                break;
            }
        }

        //int n = _currentEpisodeIndex-1;
        //if(wrapAround) {
        //    n = n >= 0 ? n : _episodeLaunchers.Length;
        //}
        //launch(n);
    }

    [ContextMenu("Launch next")]
    public void next() {
        int followingIndex = _currentEpisodeIndex+1;

        switch(loopMode) {
            case Loopmode.LoopNone: {
                launch(followingIndex);
                break;
            }
            case Loopmode.LoopAll: {
                launch(followingIndex < _episodeLaunchers.Length ? followingIndex : 0);
                break;
            }
            case Loopmode.LoopSingle: {
                launch(_currentEpisodeIndex);
                break;
            }
        }


        //if(wrapAround) {
        //    n = n < _episodeLaunchers.Length ? n : 0;
        //}

        //launch(n);
    }

    // ----------------------------------------

    // launch the first episode
    void Start()
    {
        launch(0);
    }

    void OnEnable() {
        if(prevAction) {
            prevAction.action.performed += OnPrev;
        }
        if(nextAction) {
            nextAction.action.performed += OnNext;
        }
    }

    void OnDisable() {
        if(prevAction) {
            prevAction.action.performed -= OnPrev;
        }
        if(nextAction) {
            nextAction.action.performed -= OnNext;
        }
    }

    // ----------------------------------------

    void OnNext(InputAction.CallbackContext ctx) {
        Debug.Log("onNext");
        next();
    }

    void OnPrev(InputAction.CallbackContext ctx) {
        Debug.Log("onPrev");
        prev();
    }

    // ----------------------------------------

    /**
     * 
     */
    public void launch(int i) {
        Debug.Log("launch" + i.ToString());

        if(_currentEpisode != null) {
            _currentEpisode.stop();
        }

        _currentEpisode = getEpisode(i);

        if(_currentEpisode != null) {
            _currentEpisodeIndex = getEpisodeIndex(i);
            _currentEpisode.launch();
        } else {
            _currentEpisodeIndex = -1;
        }
    }

    // maps out of range to -1
    int getEpisodeIndex(int i ) {
        return i >= 0 && i < _episodeLaunchers.Length ? i : -1;
    }

    GenericEpisode getEpisode(int i) {
        if(getEpisodeIndex(i) != -1) {
            GenericEpisode g = _episodeLaunchers[i].GetComponent<GenericEpisode>();

            if(g != null) {
                return g;
            }
        }
        
        return _defaultEpisode.GetComponent<GenericEpisode>();
    }
}
