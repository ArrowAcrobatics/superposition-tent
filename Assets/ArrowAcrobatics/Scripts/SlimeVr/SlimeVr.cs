using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeVr
{
    /**
     * Note about accepted messages towards server
     * 
     * "type": "pos"
     *      parsePosition()             // sends messages per tracker back to us.
     *      
     * "type": "action"
     *      "name": "calibrate"         // calls Main.vrServer.resetTrackersYaw();
     *      "name": "full_calibrate"    // calls Main.vrServer.resetTrackers();
     * 
     * "type": "config"                 // unhandled serverside
     * 
     */

    public enum RequestType
    {
        Position,
        Reset,
        FullReset,
        Config
    }


    [System.Serializable]
    public class Request
    {
        public string type;
        public string name;

       public  Request(RequestType t) {
            switch(t) {
                case RequestType.Position: {
                    type = "pos";
                    name = null;
                    break;
                }
                case RequestType.Reset: {
                    type = "action";
                    name = "calibrate";
                    break;
                }
                case RequestType.FullReset: {
                    type = "action";
                    name = "full_calibrate";
                    break;
                }
                case RequestType.Config: {
                    type = "config";
                    break;
                }
            }
        }
        //Request RequestPosition() {
        //    return new Request {
        //        type = "pos"
        //    };
        //}

        //Request RequestReset() {
        //    return new Request {
        //        type = "action",
        //        name = "calibrate"
        //    };
        //}

        //Request RequestFullReset() {
        //    return new Request {
        //        type = "action",
        //        name = "full_calibrate"
        //    };
        //}

        //Request RequestConfig() {
        //    return new Request {
        //        type = "config",
        //    };
        //}
        public override string ToString() { 
            return JsonUtility.ToJson(this);
        }
    }

    [System.Serializable]
    public class ResponseHeader
    {
        public string type = "";        // equal to "pos" or "config"
                                        // public string src;           // must be equal to "full". we'll ignore it. 
        public string tracker_id = "";  // "SlimeVR Tracker {i}"
        public int tracker_index;       // the {i} in the above for performance
    }

    [System.Serializable]
    public class ResponsePos
    {
        public float x, y, z;           // vec3
        public float qx, qy, qz, qw;    // quaternion
    }

    [System.Serializable]
    public class ResponseConfig
    {
        public string location = "";            // e.g. left_foot, waist, ...
        public string tracker_type = "";        // e.g. left_foot, waist, ... considered optional on serverside
    }


    
}