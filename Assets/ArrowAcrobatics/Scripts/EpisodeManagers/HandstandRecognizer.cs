using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandstandRecognizer : MonoBehaviour
{
    [System.Serializable]
    public class JointValueCollection<T>
    {
        public T leftKnee;
        public T rightKnee;
        public T leftHip;
        public T rightHip;
        public T groin;

        public T get(int i) {
            i = i%5;
            switch(i) {
                case 0:  return leftKnee;
                case 1:  return rightKnee;
                case 2:  return leftHip;
                case 3:  return rightHip;
                default: return groin;
            }
        }

        public T set(int i, T v) {
            i = i%5;
            switch(i) {
                case 0:  leftKnee  = v; return v;
                case 1:  rightKnee = v; return v;
                case 2:  leftHip   = v; return v;
                case 3:  rightHip  = v; return v;
                default: groin     = v; return v;
            }
        }

        public int Count {
            get { return 5; }
        }
    }
    
    public JointValueCollection<JointAngleTracker> joints;

    [System.Serializable]
    public class HandstandPosture {
        public string name;
        public AudioClip audioClip;

        public JointValueCollection<float> angle;
        public JointValueCollection<float> score;
        public JointValueCollection<bool> satisfied;

        public float leftKneeAngle;
        public float rightKneeAngle;
        public float leftHipAngle;
        public float rightHipAngle;
        public float groinAngle;

        public float scoreTotal;
        public bool satisfiesAll;
    }

    [Tooltip("maximum angle difference between measure and target")]
    public float thresholdDeg = 20;
    public List<HandstandPosture> handstands = new List<HandstandPosture>();


    private AudioClip _currentHandstandname = null;
    private IEnumerator audioCoroutine;
    private AudioSource _audioSource = null;

    void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }

    void OnEnable() {
        audioCoroutine = AudioPlayer();
        StartCoroutine(audioCoroutine);
    }

    void OnDisable() {
        StopCoroutine(audioCoroutine);
        if(_audioSource != null) {
            _audioSource.Stop();
        }
    }

    void Update() {
        UpdateScores();
    }

    IEnumerator AudioPlayer() {
        while(true) {
           if(_audioSource == null || _currentHandstandname == null) {
                yield return new WaitForFixedUpdate();
            } else {
                _audioSource.PlayOneShot(_currentHandstandname, 1.0f);
                yield return new WaitForSeconds(_currentHandstandname.length);
            }
        }
    }

    void UpdateScores() {
        _currentHandstandname = null;

        foreach(HandstandPosture handstand in handstands) {
            bool satisfiesAll = true; // accumulate (&=) all thresholds.
            float scoreTotal = 0; // accumulate (+=) all scores.

            // for all joints, recompute score
            for (int i = 0; i < joints.Count; i++) {
                float jointscore = 0;
                JointAngleTracker joint = joints.get(i);

                if(joint != null) {
                    jointscore = Mathf.Abs(handstand.angle.get(i) -joint.angle.val);

                    scoreTotal += handstand.score.set(i, jointscore);
                    satisfiesAll &= handstand.satisfied.set(i, jointscore < thresholdDeg);
                } else {
                    satisfiesAll &= handstand.satisfied.set(i, false);
                }
            }

            handstand.satisfiesAll = satisfiesAll;
            handstand.scoreTotal = scoreTotal;

            if (satisfiesAll) {
                _currentHandstandname = handstand.audioClip;
            }
        }
    }
}
