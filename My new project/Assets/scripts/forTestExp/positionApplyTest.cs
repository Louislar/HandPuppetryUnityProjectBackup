using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class positionApplyTest : MonoBehaviour
{
    public List<GameObject> controllJoints; // Fill this list in the Unity inspector
    public List<GameObject> rigHintJoints;
    public Vector3 curPosition;
    public float positionsIncreaseSpeed;
    [Header("Position read/apply in settings")]
    public bool isReadSynthesisPositions;
    public bool isDirectApplySynthesisPositions;
    public bool isApplyToRigJoints;
    public string readInHumanPositionFile;
    public Vector3 originHipPosition;
    public Vector3 correctOrigin;
    public int readPositionsJointsNum;
    private MediaPipeResult readInHumanPositionResult;
    private handLMsController.JointBone[] SynthesisJointsPos;

    /// <summary>
    /// 將讀取的positions apply到animation rigging的target, hint joints
    /// </summary>
    public void applyToRigJoints()
    {        
        rigHintJoints[0].transform.position = SynthesisJointsPos[1].Pos3D + originHipPosition+ correctOrigin;
        rigHintJoints[1].transform.position = SynthesisJointsPos[2].Pos3D + originHipPosition + correctOrigin;
        rigHintJoints[2].transform.position = SynthesisJointsPos[4].Pos3D + originHipPosition + correctOrigin;
        rigHintJoints[3].transform.position = SynthesisJointsPos[5].Pos3D + originHipPosition + correctOrigin;
        rigHintJoints[4].transform.position = SynthesisJointsPos[10].Pos3D + originHipPosition + correctOrigin;
        rigHintJoints[5].transform.position = SynthesisJointsPos[11].Pos3D + originHipPosition + correctOrigin;
        rigHintJoints[6].transform.position = SynthesisJointsPos[13].Pos3D + originHipPosition + correctOrigin;
        rigHintJoints[7].transform.position = SynthesisJointsPos[14].Pos3D + originHipPosition + correctOrigin;
        rigHintJoints[8].transform.position = SynthesisJointsPos[8].Pos3D + originHipPosition + correctOrigin;
        rigHintJoints[9].transform.position = SynthesisJointsPos[15].Pos3D + originHipPosition + correctOrigin;
    }

    /// <summary>
    /// 更新一次synthesis後的position data
    /// </summary>
    public void updateSynthesisPositionOnce(MediaPipeHandLMs newRots)
    {
        for (int i = 0; i < newRots.data.Count; ++i)
        {
            SynthesisJointsPos[i].Pos3D.x = newRots.data[i].x;
            SynthesisJointsPos[i].Pos3D.y = newRots.data[i].y;
            SynthesisJointsPos[i].Pos3D.z = newRots.data[i].z;
        }
    }

    /// <summary>
    /// 更新synthesis完成後的position data到avatar上
    /// </summary>
    /// <returns></returns>
    public IEnumerator updateSynthesisPosition()
    {
        int curIndex = 0;
        while (curIndex < readInHumanPositionResult.results.Length)
        {
            updateSynthesisPositionOnce(readInHumanPositionResult.results[curIndex]);
            ++curIndex;
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        curPosition = new Vector3();
        curPosition = controllJoints[2].transform.localPosition;

        // init positions array
        SynthesisJointsPos = new handLMsController.JointBone[readPositionsJointsNum];
        for (int i = 0; i < readPositionsJointsNum; ++i) { SynthesisJointsPos[i] = new handLMsController.JointBone(); }
        // Apply synthesis positions to avatar
        jsonDeserializer jsonDeserializer = new jsonDeserializer();
        readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftFrontKick_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftFrontKick_EWMA.json";
        readInHumanPositionResult = jsonDeserializer.readAndParseRotation(
            readInHumanPositionFile
        );
        if(isReadSynthesisPositions)
            StartCoroutine(updateSynthesisPosition());
    }

    // Update is called once per frame
    void Update()
    {
        if (isApplyToRigJoints)
            applyToRigJoints();
    }

    private void LateUpdate()
    {
        curPosition.x = curPosition.x + positionsIncreaseSpeed;
        controllJoints[2].transform.localPosition = curPosition;

        if (isDirectApplySynthesisPositions)
        {
            controllJoints[6].transform.position = originHipPosition + correctOrigin;
            for (int i=0; i<6;++i)
                controllJoints[i].transform.position = SynthesisJointsPos[i].Pos3D + originHipPosition + correctOrigin;
            for(int i=6; i<15;++i)
                controllJoints[i+1].transform.position = SynthesisJointsPos[i].Pos3D + originHipPosition + correctOrigin;
        }
    }
}
