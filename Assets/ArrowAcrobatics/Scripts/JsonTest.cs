using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityEngine.JSONSerializeModule.JsonUtility;

public class JsonTest : MonoBehaviour
{


    [System.Serializable]
    public class SlimeVrWebsocketRequest
    {
        public string type;
    }


    string getJsonMessage() {
        return JsonUtility.ToJson(new SlimeVrWebsocketRequest {
            type = "config"
        });
    }


    [ContextMenu("Do Something")]
    void logGeneratedJson()
    {
        Debug.Log(string.Format("{0}", getJsonMessage()));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
