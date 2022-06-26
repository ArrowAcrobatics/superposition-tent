using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStartUtils : MonoBehaviour
{
    public bool _destroy = false;
    public bool _disable = false;
    
    // Start is called before the first frame update
    void Start()
    {
        if(_destroy) {
            Destroy(transform.gameObject);
        } else if(_disable) {
            transform.gameObject.SetActive(false);
        }
    }
}
