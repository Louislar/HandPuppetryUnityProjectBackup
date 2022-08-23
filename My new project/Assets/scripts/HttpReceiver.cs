using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HttpReceiver : MonoBehaviour
{
    public bool serverOn;
    public bool updatePosToAvatar;
    public avatarController avatarController;
    public positionApplyTest positionApplyTest;
    private jsonDeserializer jsonD;

    // Start is called before the first frame update
    void Start()
    {
        if(serverOn)
        {
            StartCoroutine(ReceiveTextFromHttpServer());
            jsonD = new jsonDeserializer();
        }
    }

    IEnumerator ReceiveTextFromHttpServer()
    {
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get("localhost:8080");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                // json text to data
                MediaPipeResult handRotation = JsonUtility.FromJson<MediaPipeResult>(
                    "{\"results\":" + www.downloadHandler.text + "}"
                    );

                // update position data to avatar
                if (updatePosToAvatar)
                    //avatarController.updateSynthesisPositionOnce(handRotation.results[0]);
                    positionApplyTest.updateSynthesisPositionOnce(handRotation.results[0]);

                // Or retrieve results as binary data
                byte[] results = www.downloadHandler.data;
            }
            //yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(0.05f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
