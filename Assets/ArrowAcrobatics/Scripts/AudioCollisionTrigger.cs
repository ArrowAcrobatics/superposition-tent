using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioCollisionTrigger : MonoBehaviour
{
    private AudioSource _audioSource = null;

    //Play the music
    private bool _prevPlay = false;
    public bool _play = false;

    void OnEnable() {
        Debug.Log("enable");
        _audioSource = GetComponent<AudioSource>();
        _play = _audioSource.isPlaying;
        _prevPlay = _audioSource.isPlaying;
    }

    // OnDisable will already disable audio, no need to update.

    void Update() {
        if(_play == _prevPlay) {
            // no state change
            return;
        }

        // latch
        _prevPlay = _play;

        if(_play) {
            //Play the audio you attach to the AudioSource component
            _audioSource.Play();
        } else {
            _audioSource.Stop();
        }
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("triggered");
        _play = !_play;
    }
}