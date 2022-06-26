using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyConditions : MonoBehaviour
{
    [Range(0.0f,300.0f)]
    public float _timeOut = 1.0f;

    [Tooltip("does not take timeout into account")]
    public bool _disableOnEnable = false;

    [Tooltip("takes timeout into account")]
    public bool _destroyOnEnable = false;
    
    [Tooltip("takes timeout into account")]
    public bool _destroyOnTriggerEnter = true;

    void OnEnable() {
        if(_destroyOnEnable) {
            Destroy(gameObject, _timeOut);
        }
        if(_disableOnEnable) {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other) {
        if(_destroyOnTriggerEnter) {
            Destroy(gameObject, _timeOut);
        }
    }
}
