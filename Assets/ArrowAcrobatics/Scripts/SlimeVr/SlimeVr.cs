using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SlimeVrWebsocketRequest
{
    public string type;
    public string name;
}

[System.Serializable]
public class SlimeVrWebsocketResponseHeader
{
    public string type = "";        // equal to "pos" or "config"
                                    // public string src;           // must be equal to "full". we'll ignore it. 
    public string tracker_id = "";  // "SlimeVR Tracker {i}"
    public int tracker_index;       // the {i} in the above for performance
}

[System.Serializable]
public class SlimeVrWebsocketResponsePos
{
    public float x, y, z;           // vec3
    public float qx, qy, qz, qw;    // quaternion
}

[System.Serializable]
public class SlimeVrWebsocketResponseConfig
{
    public string location = "";            // e.g. left_foot, waist, ...
    public string tracker_type = "";        // e.g. left_foot, waist, ... considered optional on serverside
}
