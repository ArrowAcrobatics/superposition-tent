using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Mover : MonoBehaviour
{
    Rigidbody _rigidbody = null;

    public Vector3 direction = Vector3.zero;
    public float speed = 1.0f;

    void Start() {
        _rigidbody = GetComponent<Rigidbody>();
        direction.Normalize();
    }

    void FixedUpdate () {
        if(_rigidbody.isKinematic) {
            _rigidbody.MovePosition(transform.position + direction * (Time.deltaTime* speed));
        } else {
            _rigidbody.velocity = direction * speed;
        }
    }
}
