using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandstandRecognizer : MonoBehaviour
{
    [System.Serializable]
    public class HandstandPosture {
        public string name;
        public AudioClip audioClip;

        public float leftKneeAngle;
        public float rightKneeAngle;
        public float leftHipAngle;
        public float rightHipAngle;
        public float groinAngle;

        public float score;
        public bool satisfiesThresholds;
    }

    public JointAngleTracker leftKnee = null;
    public JointAngleTracker rightKnee = null;
    public JointAngleTracker leftHip = null;
    public JointAngleTracker rightHip = null;
    public JointAngleTracker groin = null;

    [Tooltip("maximum angle difference between measure and target")]
    public float thresholdDeg = 20;
    public List<HandstandPosture> handstands = new List<HandstandPosture>();

    void Update() {
        UpdateScores();
    }

    void UpdateScores() {
        foreach(HandstandPosture handstand in handstands) {
            bool satisfied = true;
            float totalScore = 0;
            float jointscore = 0;

            if(leftKnee.angle  != null) {
                jointscore = Mathf.Abs(handstand.leftKneeAngle  -leftKnee.angle.val);
                totalScore += jointscore*jointscore;
                satisfied &= jointscore < thresholdDeg;
            } else { 
                satisfied = false;
            }

            if(rightKnee.angle  != null) {
                jointscore = Mathf.Abs(handstand.rightKneeAngle  -rightKnee.angle.val);
                totalScore += jointscore*jointscore;
                satisfied &= jointscore < thresholdDeg;
            } else {
                satisfied = false;
            }

            if(leftHip.angle  != null) {
                jointscore = Mathf.Abs(handstand.leftHipAngle  -leftHip.angle.val);
                totalScore += jointscore*jointscore;
                satisfied &= jointscore < thresholdDeg;
            } else {
                satisfied = false;
            }

            if(rightHip.angle  != null) {
                jointscore = Mathf.Abs(handstand.rightHipAngle  -rightHip.angle.val);
                totalScore += jointscore*jointscore;
                satisfied &= jointscore < thresholdDeg;
            } else {
                satisfied = false;
            }

            if(groin.angle  != null) {
                jointscore = Mathf.Abs(handstand.groinAngle  -groin.angle.val);
                totalScore += jointscore*jointscore;
                satisfied &= jointscore < thresholdDeg;
            } else {
                satisfied = false;
            }

            handstand.satisfiesThresholds = satisfied;
            handstand.score = totalScore;
        }
    }
}
