using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rotator : MonoBehaviour
{
    public float speedDeg_S;
    public Vector3 axis = Vector3.up;

    public bool randomAxis = false;

    Rigidbody _rigidbody = null;    

    void OnEnable() {
        _rigidbody = GetComponent<Rigidbody>();
        if(randomAxis) {
            axis = Random.insideUnitSphere;
            if(axis == Vector3.zero) {
                axis = Vector3.up;
            }
        }
        axis.Normalize();
    }

    void FixedUpdate()
    {
        float deltaAngle = speedDeg_S * Time.deltaTime;

        if(_rigidbody != null) {
            if(_rigidbody.isKinematic) {
                _rigidbody.MoveRotation(Quaternion.AngleAxis(deltaAngle, axis) * transform.rotation);
            } else {
                _rigidbody.angularVelocity = axis * speedDeg_S; //TODO: actual fix... this is a shortcut
                //Quaternion.AngleAxis(deltaAngle, axis).eulerAngles;
            }
        } else {
            transform.Rotate(axis, deltaAngle);
        }
    }
}
