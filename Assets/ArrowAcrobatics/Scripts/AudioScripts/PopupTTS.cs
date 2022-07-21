using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class PopupTTS : MonoBehaviour
{
    // dont change at runtime please :)
    [SerializeField]
    private PopupTTSFragment[] _fragments;
    private AudioSource _source;

    public float _initialWait = 0.0f;
    public float _finalWait = 0.0f;
    
    private IEnumerator _playbackRoutineHandle = null;
    private int _currentFragmentIndex = 0;
    private int _nextFragmentIndex = 0;

    public bool _gotoNextEpisodeOnExit = true;
    public bool _launchOnEnable = true;

    // ----------------------------------------

    [SerializeField] InputActionReference prevAction;
    [SerializeField] InputActionReference nextAction;

    void OnNext(InputAction.CallbackContext ctx) {
        Debug.Log("onNext");
        next();
    }

    void OnPrev(InputAction.CallbackContext ctx) {
        Debug.Log("onPrev");
        prev();
    }

    // ----------------------------------------

    // (re)starts playback routine
    void OnEnable() {
        if(prevAction) {
            prevAction.action.performed += OnPrev;
        }
        if(nextAction) {
            nextAction.action.performed += OnNext;
        }

        _source = GetComponent<AudioSource>();

        if(_source == null) {
            return;
        }

        if (_fragments.Length == 0) {
            return;
        }

        if(_launchOnEnable) {
            bool firstCall = _currentFragmentIndex == 0;
            relaunchRoroutine(firstCall);
        }
    }


    // stops routine and playback of current clip
    void OnDisable() {
        if(_playbackRoutineHandle != null) {
            StopCoroutine(_playbackRoutineHandle);
            _source.clip = null;
        }
        _playbackRoutineHandle = null;


        if(prevAction) {
            prevAction.action.performed -= OnPrev;
        }
        if(nextAction) {
            nextAction.action.performed -= OnNext;
        }
    }

    void next() {
        if(_playbackRoutineHandle != null) {
            // only update next if we are already running
            setNextFragment();
        }
        relaunchRoroutine(false);
    }

    void prev() {
        if(_playbackRoutineHandle != null) {
            // only update prev if we are already running
            setPrevFragment();
        }
        relaunchRoroutine(false);
    }

    void setNextFragment() {
        _nextFragmentIndex = _currentFragmentIndex + 1;
    }

    void setPrevFragment() {
        _nextFragmentIndex = _currentFragmentIndex - 1;
    }


    void relaunchRoroutine(bool firstCall) {
        if(_playbackRoutineHandle != null) {
            StopCoroutine(_playbackRoutineHandle);
        }

        _playbackRoutineHandle = playbackRoutine(firstCall);

        StartCoroutine(_playbackRoutineHandle);
    }

    IEnumerator playbackRoutine(bool applyInitialWait) {
        if(_initialWait > 0 && applyInitialWait) {
            yield return new WaitForSeconds(_initialWait);
        }

        // while the current index is in range
        while (_currentFragmentIndex < _fragments.Length && _currentFragmentIndex >= 0) {
            PopupTTSFragment currentFragment = _fragments[_currentFragmentIndex];

            // play the clip
            StopPrevClip();
            if(currentFragment._clip != null) {
                _source.clip = currentFragment._clip;
                _source.Play();
                float duration = currentFragment._duration;
                if(currentFragment._addClipLengthToDuration) {
                    duration += currentFragment._clip.length;
                }
                yield return new WaitForSeconds(duration);
            }

            // finished playing current fragment, so we go to next or wait for user to make a change.
            if(!currentFragment._waitForUserinput) {
                setNextFragment();
            }

            // finished playing this fragment, waiting until next fragment index is updated.
            while(_currentFragmentIndex == _nextFragmentIndex) {
                yield return new WaitForFixedUpdate();
            }

            /// change happened, updating current index.
            _currentFragmentIndex = _nextFragmentIndex;
        }

        yield return new WaitForSeconds(_finalWait);

        if(_gotoNextEpisodeOnExit) {
            EpisodeManager epiMan = FindObjectOfType<EpisodeManager>();
            epiMan.next();
        }

        yield return null;
    }

    void StopPrevClip() {
        _source.Stop();

    }
}
