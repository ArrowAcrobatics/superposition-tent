using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

using NativeWebSocket;

/**
 * Establishes connection with SlimeVr server and parses messages.
 */
public class SlimeVrClient : MonoBehaviour
{    
    public int portno;
    public SlimeVr.RequestType requestType = SlimeVr.RequestType.Position;
    public bool liveUpdatePosition = true;
    public bool verboseLogging = false;

    [Tooltip("this is where the tracker prefabs will spawn into")]
    public GameObject SlimeSkeleton;
    public GameObject trackerPrefab;
    public List<GameObject> trackerObjects = new List<GameObject>();
    private List<SlimeVrTracker> trackerComponents = new List<SlimeVrTracker>();


    [SerializeField] InputActionReference resetTrackers;
    [SerializeField] InputActionReference resetTrackersFully;

    private WebSocket websocket;

    #region Unity block


    void Awake() {
        trackerComponents = SlimeSkeleton.GetComponentsInChildren<SlimeVrTracker>().ToList();
    }

    void Update() {
#if !UNITY_WEBGL || UNITY_EDITOR
        if(websocket != null) {
            websocket.DispatchMessageQueue();
        }
#endif
    }

    void FixedUpdate() {
        if(liveUpdatePosition) {
            SendWebSocketMessage(new SlimeVr.Request(requestType));
        }
    }

    private void OnApplicationQuit() {
        closeWebsocket();
    }

    void OnEnable() {
        openWebsocket();

        if(resetTrackers) {
            resetTrackers.action.performed += ResetTrackers;
        }
        if(resetTrackersFully) {
            resetTrackersFully.action.performed += ResetTrackersFully;
        }
    }

    void OnDisable() {
        closeWebsocket();

        if(resetTrackers) {
            resetTrackers.action.performed -= ResetTrackers;
        }
        if(resetTrackersFully) {
            resetTrackersFully.action.performed -= ResetTrackersFully;
        }
    }

    void ResetTrackers(InputAction.CallbackContext ctx) {
        Debug.Log("ResetTrackers()");
        SendWebSocketMessage(SlimeVr.RequestType.Reset);
    }

    void ResetTrackersFully(InputAction.CallbackContext ctx) {
        Debug.Log("ResetTrackersFully()");
        SendWebSocketMessage(SlimeVr.RequestType.FullReset);
    }


    #endregion


    #region websocket related stuff

    /**
     * Adjusts position of the relevant tracker gameobject using the tracker_index.
     * 
     * assumes header.type == "pos"
     * msg is the full incoming message
     */
    void HandlePosMessage(SlimeVr.ResponseHeader header, string msg) {
        SlimeVr.ResponsePos pos = JsonUtility.FromJson<SlimeVr.ResponsePos>(msg);

        GameObject trackerObject = trackerObjects.ElementAtOrDefault(header.tracker_index);
        if (trackerObject != null) {
            trackerObject.transform.position = new Vector3(pos.x,pos.y,pos.z);
            trackerObject.transform.rotation = new Quaternion(pos.qx, pos.qy, pos.qz, pos.qw);
        }
    }

    /**
     * Creates an instance of trackerPrefab as child of this gameobject and 
     * adds reference in the trackerObjects array at the given index.
     * 
     * assumes header.type == "config".
     * msg is the full incoming message.
     */
    void HandleConfigMessage(SlimeVr.ResponseHeader header, string msg) {
        SlimeVr.ResponseConfig conf = JsonUtility.FromJson<SlimeVr.ResponseConfig>(msg);

        GameObject g = null;
        foreach(SlimeVrTracker tracker in trackerComponents) {
            if (tracker.gameObject.name == conf.location) {
                g = tracker.gameObject;
                break;
            }
        }
        //Transform t = SlimeSkeleton.transform.Find(conf.location);
        //GameObject g = t != null ? t.gameObject : null;

        // if necessary, create gameobject with SlimeVr location as name (using prefab)
        //if(g == null) {
        //    if(trackerPrefab != null) {
        //        g = Instantiate(trackerPrefab, SlimeSkeleton.transform, false) as GameObject;
        //        g.name = conf.location;
        //    } else {
        //        g = new GameObject(conf.location);
        //        g.transform.SetParent(SlimeSkeleton.transform, false); // keep position at (0,0,0) by setting worldPositionStays to false.
        //    }
        //}

        // add tracker to our list at correct index
        EnsureIndexExists(header.tracker_index);
        trackerObjects[header.tracker_index] = g;
    }

    /**
     * Expands trackerObjects array with null objects to accomodate for given index.
     */
    void EnsureIndexExists(int i) {
        int diff = i - trackerObjects.Count;
        if(diff >= 0) {
            Debug.Log(string.Format("adding {0} elements to list of count {1} to accomodate for index {2}", diff+1, trackerObjects.Count, i));
            GameObject[] nulls = new GameObject[diff+1];
            trackerObjects.AddRange(nulls);
        }
    }
    
    [ContextMenu("Send request")]
    void sendCurrentRequest() {
        SendWebSocketMessage(new SlimeVr.Request(requestType));
    }

    // Start is called before the first frame update
    [ContextMenu("open websocket")]
    async void openWebsocket() {
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
            if(verboseLogging) {
                Debug.Log(string.Format("onMessage: {0}", msg));
            }

            SlimeVr.ResponseHeader h = JsonUtility.FromJson<SlimeVr.ResponseHeader>(msg);
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

    void SendWebSocketMessage(SlimeVr.RequestType reqType) {
        SendWebSocketMessage(new SlimeVr.Request(reqType));
    }

    async void SendWebSocketMessage(SlimeVr.Request req ) {
        if(websocket != null && websocket.State == WebSocketState.Open) {
            // Sending plain text
            string msg = req.ToString();

            if (verboseLogging) {
                Debug.Log(string.Format("sending: {0}", msg));
            }
            await websocket.SendText(msg);
        }
    }
    #endregion
}