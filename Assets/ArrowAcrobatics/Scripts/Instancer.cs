using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instancer : MonoBehaviour
{
    public GameObject _prefab = null;

    [Tooltip("instances will be parented to this")]
    public Transform _parent = null;

    [Tooltip("in seconds per instance, negative will not instance")]
    public float _period;

    // in seconds, first instance is on first update call.
    private float _timePassedSincelastInstance = float.PositiveInfinity;

    // Update is called once per frame
    void Update()
    {
        if(_timePassedSincelastInstance > _period && _period > 0 && _prefab != null) {
            // use our transform as parent to prevent havoc in game object tree
            Instantiate(_prefab, _parent != null ? _parent : transform);
            _timePassedSincelastInstance = 0;
        }

        _timePassedSincelastInstance+= Time.deltaTime;
    }
}
