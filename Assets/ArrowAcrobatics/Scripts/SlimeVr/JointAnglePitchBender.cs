using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JointAngleTracker))]
[RequireComponent(typeof(AudioSource))]
public class JointAnglePitchBender : MonoBehaviour
{
    [Range(0, 180)]
    public float angleMin = 0;
    [Range(0, 180)]
    public float angleMax = 180;

    [Range(0, 4)]
    public float pitchMin = 1;
    [Range(0, 4)]
    public float pitchMax = 2;
    
    private JointAngleTracker joint;
    private AudioSource audioSource;

    void Start()
    {
        joint = GetComponent<JointAngleTracker>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(joint.angle != null) {
            audioSource.pitch = Mathf.Lerp(pitchMin, pitchMax,
                Mathf.InverseLerp(angleMin, angleMax, joint.angle)
            );
        }
    }
}
