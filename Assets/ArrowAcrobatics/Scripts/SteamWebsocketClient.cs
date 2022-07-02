using System.Collections;
using System.Collections.Generic;
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

    public GameObject[] trackedObjects;
    

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


    public SlimeVrWebsocketResponsePos CurrentPos = new SlimeVrWebsocketResponsePos();
    public SlimeVrWebsocketResponseConfig CurrentConf = new SlimeVrWebsocketResponseConfig();


    /**
    * assumes header.type == "pos"
    * msg is the full incoming message
    */
    void HandlePosMessage(SlimeVrWebsocketResponseHeader head, string msg) {
        JsonUtility.FromJsonOverwrite(msg, CurrentPos);
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
            
            if(trackerPrefab != null) {
                Object g = Instantiate(trackerPrefab, transform, false);
                g.name = conf.location;
            } else {
                GameObject g = new GameObject(conf.location);
                g.transform.SetParent(transform, false); // keep position at (0,0,0) by setting worldPositionStays to false.
            }
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
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            Debug.Log(e);
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

        // Keep sending messages at every 0.3s
        //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

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