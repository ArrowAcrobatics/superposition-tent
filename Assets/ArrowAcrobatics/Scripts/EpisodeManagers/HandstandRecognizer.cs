using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandstandRecognizer : MonoBehaviour
{
    [System.Serializable]
    public struct HandstandPosture {
        public string name;
        public AudioClip audioClip;

        public float leftKneeAngle;
        public float rightKneeAngle;
        public float leftHipAngle;
        public float rightHipAngle;
        public float groinAngle;
    }

    public List<HandstandPosture> handstands = new List<HandstandPosture>();

}
