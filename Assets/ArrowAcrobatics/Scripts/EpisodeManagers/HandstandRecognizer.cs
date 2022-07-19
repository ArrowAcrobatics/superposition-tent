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

    public JointAngleTracker leftKnee = null;
    public JointAngleTracker rightKnee = null;
    public JointAngleTracker leftHip = null;
    public JointAngleTracker rightHip = null;
    public JointAngleTracker groin = null;

    [Tooltip("maximum angle difference between measure and target")]
    public float thresholdDeg = 20;
    public List<HandstandPosture> handstands = new List<HandstandPosture>();
}
