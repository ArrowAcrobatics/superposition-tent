using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct PopupTTSFragment {
    public AudioClip _clip;
    public string _text;
    public float _duration;
    public bool _waitForUserinput;
}
