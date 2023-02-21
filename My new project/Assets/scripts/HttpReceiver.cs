using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HttpReceiver : MonoBehaviour
{
    public bool serverOn;
    public bool updatePosToAvatar;
    public bool updateWristPosToAvatarRootPos;
    public avatarController avatarController;
    public positionApplyTest positionApplyTest;
    public List<string> accessURLs;
    private jsonDeserializer jsonD;

    // Start is called before the first frame update
    void Start()
    {
        // 不斷交替對不同url發出請求 
        //accessURLs = new List<string> { "localhost:8080", "localhost:8080/wrist" };
        accessURLs = new List<string> {
            "localhost:8080/", "localhost:8080/wrist"
        };
        if (serverOn)
        {
            StartCoroutine(ReceiveTextFromHttpServer());
            jsonD = new jsonDeserializer();
        }
    }

    IEnumerator ReceiveTextFromHttpServer()
    {
        int urlIndex = 0;
        while (true)
        {
            UnityWebRequest www;
            if (urlIndex==0)
            {
                www = UnityWebRequest.Get(accessURLs[urlIndex] + positionApplyTest.actionType);
            }
            else
            {
                www = UnityWebRequest.Get(accessURLs[urlIndex]);
            }
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
                if (updatePosToAvatar & urlIndex==0)
                    //avatarController.updateSynthesisPositionOnce(handRotation.results[0]);
                    positionApplyTest.updateSynthesisPositionOnce(handRotation.results[0]);
                else if(updateWristPosToAvatarRootPos & urlIndex == 1)
                {
                    // TODO: update wrist position to avatar root positions
                    positionApplyTest.updateRootPositionOnce(handRotation.results[0]);
                }

                // update URL index for switching between body pose and hand pose 
                urlIndex++;
                urlIndex = urlIndex % accessURLs.Count;

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
