using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * After initial delay, starts spawning the prefab at given frequency
 * reducing it slowly to a minimum
 */
public class PopupSpawner : MonoBehaviour
{
    public GameObject _popUp = null;

    [Tooltip("time until the first spawn")]
    public float _initWait = 10.0f;
    [Tooltip("go to next scene after timeout expires.")]
    public float _gotoNextTimeout = 0;

    [Tooltip("period between spawns at time of the first spawn")]
    public float _spawnWaitStart = 2.0f;
    
    [Tooltip("minimum period between spawns")]
    public float _spawnWaitMin = 2.0f;
    
    [Tooltip("reduction factor between consequtive spawns")]
    [Range(0.0f,1.0f)]
    public float _spawnWaitReduceFactor = 1.0f;

    public Vector3 _randMin = Vector3.zero;
    public Vector3 _randMax = Vector3.zero;

    private bool _initialized = false;
    private float _spawnWait;

    void OnEnable() {
        StartCoroutine(SpawnRoutine());

        if(_gotoNextTimeout > 0) {
            StartCoroutine(GotoNextAfterDelay(_gotoNextTimeout));
        }
    }

    IEnumerator GotoNextAfterDelay(float delaySeconds) {
        Debug.Log("going to next scene after " + delaySeconds.ToString());
        yield return new WaitForSeconds(delaySeconds);

        EpisodeManager epiMan = FindObjectOfType<EpisodeManager>();
        epiMan.next();
    }

    IEnumerator SpawnRoutine() {
        while(true) {
            if(!_initialized) {
                _initialized = true;
                Debug.Log("initing spawner");

                _spawnWait = _spawnWaitStart;
                yield return new WaitForSeconds(_initWait);

                // first spawn is fixed position
                Spawn(false);
            }
            
            yield return new WaitForSeconds(_spawnWait);
            Spawn(true);

            _spawnWait = Mathf.Max(_spawnWait * _spawnWaitReduceFactor, _spawnWaitMin);
        }
    }

    void Spawn(bool rand) {
        if(rand) {
            Vector3 position = new Vector3(
                Random.Range(_randMin.x, _randMax.x) + gameObject.transform.position.x,
                Random.Range(_randMin.y, _randMax.y) + gameObject.transform.position.y, 
                Random.Range(_randMin.z, _randMax.z) + gameObject.transform.position.z);

            Instantiate(_popUp, position, Quaternion.identity, gameObject.transform);
        } else {
            Instantiate(_popUp, gameObject.transform);
        }
    }
}
