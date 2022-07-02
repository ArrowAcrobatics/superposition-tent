using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using NativeWebSocket;

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
public class SteamWebsocketClient : MonoBehaviour
{
    WebSocket websocket;
    public int portno;
    public string request;
    public GameObject trackerPrefab;

    public List<GameObject> trackerObjects = new List<GameObject>();
    

    [System.Serializable]
    public class SlimeVrWebsocketRequest {
        public string type;
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
    public class SlimeVrWebsocketResponsePos {
        public float x, y, z;           // vec3
        public float qx, qy, qz, qw;    // quaternion
    }

    [System.Serializable]
    public class SlimeVrWebsocketResponseConfig {
        public string location = "";            // e.g. left_foot, waist, ...
        public string tracker_type = "";        // e.g. left_foot, waist, ... considered optional on serverside
    }

    /**
    * assumes header.type == "pos"
    * msg is the full incoming message
    */
    void HandlePosMessage(SlimeVrWebsocketResponseHeader header, string msg) {
        SlimeVrWebsocketResponsePos pos = JsonUtility.FromJson<SlimeVrWebsocketResponsePos>(msg);

        GameObject trackerObject = trackerObjects.ElementAtOrDefault(header.tracker_index);
        if (trackerObject != null) {
            trackerObject.transform.position = new Vector3(pos.x,pos.y,pos.z);
        }
    }

    /**
     * assumes header.type == "config"
     * msg is the full incoming message
     * 
     */
    void HandleConfigMessage(SlimeVrWebsocketResponseHeader header, string msg) {
        SlimeVrWebsocketResponseConfig conf = JsonUtility.FromJson<SlimeVrWebsocketResponseConfig>(msg);

        // creates gameobject with SlimeVr location as name
        Transform t = transform.Find(conf.location);
        if(t == null) {
            GameObject g = null;
            if(trackerPrefab != null) {
                g = Instantiate(trackerPrefab, transform, false) as GameObject;
                g.name = conf.location;
            } else {
                g = new GameObject(conf.location);
                g.transform.SetParent(transform, false); // keep position at (0,0,0) by setting worldPositionStays to false.
            }

            // add tracker to our list at correct index
            int diff = header.tracker_index - trackerObjects.Count;
            if(diff >= 0) {
                Debug.Log(string.Format("adding {0} elements to list of count {1} to accomodate for index {2}", diff+1, trackerObjects.Count, header.tracker_index));
                GameObject[] nulls = new GameObject[diff+1];
                trackerObjects.AddRange(nulls);
            }
            trackerObjects[header.tracker_index] = g;
        }
    }
    
    [ContextMenu("Send request")]
    void logGeneratedJson() {
        SendWebSocketMessage();
    }

    // Start is called before the first frame update
    [ContextMenu("open websocket")]
    async void open() {
        websocket = new WebSocket(string.Format("ws://localhost:{0}", portno));

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e.ToString());
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!" + e.ToString());
        };

        websocket.OnMessage += (bytes) =>
        {
            string msg = System.Text.Encoding.Default.GetString(bytes);
            Debug.Log(string.Format("onMessage: {0}", msg));

            SlimeVrWebsocketResponseHeader h = JsonUtility.FromJson<SlimeVrWebsocketResponseHeader>(msg);
            switch(h.type) {
                case "pos":
                    HandlePosMessage(h, msg);
                    break;
                case "config":
                    HandleConfigMessage(h, msg);
                    break;
                default:
                    Debug.LogWarning("unhandled message");
                    break;
            }
        };

        // waiting for messages
        await websocket.Connect();
    }

    [ContextMenu("close socket")]
    async void closeWebsocket() {
        if(websocket != null) {
            await websocket.Close();
        }
    }

    void Update() {
#if !UNITY_WEBGL || UNITY_EDITOR
        if(websocket != null) {
            websocket.DispatchMessageQueue();
        }
#endif
    }

    async void SendWebSocketMessage() {
        if(websocket.State == WebSocketState.Open) {
            // Sending plain text
            string msg = JsonUtility.ToJson(new SlimeVrWebsocketRequest {
                type = request
            });

            Debug.Log(string.Format("sending: {0}", msg));
            await websocket.SendText(msg);
        }
    }

    private void OnApplicationQuit() {
        closeWebsocket();
    }
}