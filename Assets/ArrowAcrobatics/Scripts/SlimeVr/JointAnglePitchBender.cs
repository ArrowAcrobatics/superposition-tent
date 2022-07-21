using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class JointAnglePitchBender : MonoBehaviour
{
    [Tooltip("The joint to track the angle of. If null it will try to GetComponent<>")]
    public JointAngleTracker joint;

    [Range(0, 180)]
    public float angleMin = 0;
    [Range(0, 180)]
    public float angleMax = 180;

    [Range(0, 4)]
    public float pitchMin = 1;
    [Range(0, 4)]
    public float pitchMax = 2;

    public bool applyAngleSmoothing;
    public bool applyAngularSpeedSmoothing;

    public enum PitchMode {
        Continuous,
        TwelveTone,
        Diatonic,
        Pentatonic
    }
    public PitchMode pitchMode;

    public bool DiscretizeTones = false;
    public float relativeAngleDebug;
    public float rounded12toneAngleDebug;


    public enum VolumeMode
    {
        ConstantMin,
        ConstantMax,
        AngularSpeedThresholdMin,
        AngularSpeedThresholdMax,
        AngularSpeed
    }

    public VolumeMode volumeMode;

    [Tooltip("Volume is set as function of the speed, volumeMax is attained when angular speed is bigger than this val.")]
    public float volumeMinAngularSpeed = 0;
    public float volumeMaxAngularSpeed = 180;
    [Range(0,1)]
    [Tooltip("Higher is smoother but slower to react")]
    public float volumeSmoothingFactor = 0.1f;

    [Range(0,1)]
    public float volumeMin = 0;
    [Range(0, 1)]
    public float volumeMax = 1;

    private JointAngleTracker.NullableFloat prevAngle = null;
    private JointAngleTracker.NullableFloat prevAngularSpeed = null;

    private AudioSource audioSource;

    void Start()
    {
        if(joint == null) {
            joint = GetComponent<JointAngleTracker>();
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(joint.angle != null) {
            float angle = joint.angle;           

            if(prevAngle != null) {
                if(applyAngleSmoothing) {
                    angle = Mathf.Lerp(angle, prevAngle, volumeSmoothingFactor);
                }
                
                float angularSpeed = Mathf.Abs(((float)joint.angle - (float)prevAngle)) / Time.deltaTime;

                if(applyAngularSpeedSmoothing && prevAngularSpeed != null) {
                    angularSpeed = Mathf.Lerp(angularSpeed, prevAngularSpeed, volumeSmoothingFactor);
                }

                switch(volumeMode) {
                    case VolumeMode.ConstantMin: {
                        audioSource.volume = volumeMin;
                        break;
                    }
                    case VolumeMode.ConstantMax: {
                        audioSource.volume = volumeMax;
                        break;
                    }
                    case VolumeMode.AngularSpeedThresholdMin: {
                        audioSource.volume = angularSpeed < volumeMinAngularSpeed ? volumeMin : volumeMax;
                        break;
                    }
                    case VolumeMode.AngularSpeedThresholdMax: {
                        audioSource.volume = angularSpeed < volumeMaxAngularSpeed ? volumeMin : volumeMax;
                        break;
                    }
                    case VolumeMode.AngularSpeed: {
                        
                        audioSource.volume = Mathf.Lerp(volumeMin, volumeMax,
                            Mathf.InverseLerp(volumeMinAngularSpeed, volumeMaxAngularSpeed, angularSpeed)
                        );
                        break;
                    }
                }

                prevAngularSpeed = angularSpeed;
            }

            audioSource.pitch = getPitch(angle);
        }

        if(joint.angle == null || prevAngle == null) {
            // can't compute angular speed without either value
            audioSource.volume = volumeMin;
            prevAngularSpeed = null;
        }
         
        prevAngle = joint.angle;
    }

    float getPitch(float angle) {
        float relativeAngle = Mathf.InverseLerp(angleMin, angleMax, angle);
        relativeAngleDebug = relativeAngle;

        if (pitchMode == PitchMode.Continuous) {
            return Mathf.Lerp(pitchMin, pitchMax, relativeAngle);
        }

        int rounded12toneAngle = (int)(relativeAngle * 12); // includes the octave
        if(rounded12toneAngle < 0 ) { rounded12toneAngle  = 0; }
        if(rounded12toneAngle > 12) { rounded12toneAngle  = 12; }

        switch(pitchMode) {
            case PitchMode.TwelveTone: {
                return pitchMin * Mathf.Pow(2, rounded12toneAngle/12.0f);
            }
            case PitchMode.Diatonic: {
                int[] diatonicMap = { 0, 0, 2, 2, 4, 5, 5, 7, 7, 9, 9, 11, 12 };
                rounded12toneAngle = diatonicMap[rounded12toneAngle];
                break;
            }
            case PitchMode.Pentatonic: {
                int[] pentatonicMap = { 1, 1, 3, 3, 3, 6, 6, 8, 8, 10, 10, 10, 13 };
                rounded12toneAngle = pentatonicMap[rounded12toneAngle];
                break;
            }
        }
        rounded12toneAngleDebug = rounded12toneAngle;

        return pitchMin * Mathf.Pow(2, rounded12toneAngle/12.0f);
    }
}
