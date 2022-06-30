using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;

public class SteamWebsocketClient : MonoBehaviour
{
    WebSocket websocket;
    public int portno;
    public string request;

    [System.Serializable]
    public class SlimeVrWebsocketRequest
    {
        public string type;
    }


    string getJsonMessage() {
        return JsonUtility.ToJson(new SlimeVrWebsocketRequest {
            type = request
        });
    }


    [ContextMenu("Send request")]
    void logGeneratedJson() {
        Debug.Log(string.Format("{0}", getJsonMessage()));
        SendWebSocketMessage();
    }

    IEnumerator spammer() {
        yield return new WaitForSeconds(0.3f);
        logGeneratedJson();
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
            Debug.Log(string.Format("onMessage: {0}", System.Text.Encoding.Default.GetString(bytes)));

            // getting the message as a string
            // var message = System.Text.Encoding.UTF8.GetString(bytes);
            // Debug.Log("OnMessage! " + message);
        };

        // Keep sending messages at every 0.3s
        //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        // waiting for messages
        await websocket.Connect();

        //StartCoroutine(spammer());
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
            // Sending bytes
            //await websocket.Send(new byte[] { 10, 20, 30 });

            // Sending plain text
            string msg = getJsonMessage();
            Debug.Log(string.Format("sending: {0}", msg));
            await websocket.SendText(msg);
        }
    }

    private void OnApplicationQuit() {
        closeWebsocket();
    }

}