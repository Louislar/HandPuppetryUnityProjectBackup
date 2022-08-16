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
    public bool isApplySynthesisRotations;
    public bool isApplyToRigJoints;
    public string readInHumanPositionFile;
    public Vector3 originHipPosition;
    public Vector3 correctOrigin;
    public int readPositionsJointsNum;
    private MediaPipeResult readInHumanPositionResult;
    private handLMsController.JointBone[] SynthesisJointsPos;

    /// <summary>
    /// TODO: 計算各個joints的rotation, 理論上只需要計算雙手與雙腳的各兩個joint的旋轉即可
    ///         目前計算結果apply到avatar身上，還是與直接apply position有一些落差
    /// </summary>
    public void computeRigJointsRotAndApply()
    {
        Debug.DrawRay(originHipPosition + correctOrigin, SynthesisJointsPos[0].Pos3D, Color.green);
        // Hip
        controllJoints[6].transform.rotation =
            Quaternion.FromToRotation(new Vector3(-0.08207779f, -0.06751716f, -0.01599556f), SynthesisJointsPos[0].Pos3D);
        // Left upper leg
        controllJoints[0].transform.rotation =
            Quaternion.FromToRotation(new Vector3(0, -1, 0), SynthesisJointsPos[1].Pos3D - SynthesisJointsPos[0].Pos3D);
        // Left knee
        controllJoints[1].transform.rotation =
            Quaternion.FromToRotation(new Vector3(0, -1, 0), SynthesisJointsPos[2].Pos3D- SynthesisJointsPos[1].Pos3D);
        // Right upper leg
        controllJoints[3].transform.rotation =
            Quaternion.FromToRotation(new Vector3(0, -1, 0), SynthesisJointsPos[4].Pos3D- SynthesisJointsPos[3].Pos3D);
        // Right knee
        controllJoints[4].transform.rotation =
            Quaternion.FromToRotation(new Vector3(0, -1, 0), SynthesisJointsPos[5].Pos3D- SynthesisJointsPos[4].Pos3D);
        // Left upper arm
        controllJoints[10].transform.rotation =
            Quaternion.FromToRotation(new Vector3(-1, 0, 0), SynthesisJointsPos[10].Pos3D- SynthesisJointsPos[9].Pos3D);
        // Left elbow
        controllJoints[11].transform.rotation =
            Quaternion.FromToRotation(new Vector3(-1, 0, 0), SynthesisJointsPos[11].Pos3D- SynthesisJointsPos[10].Pos3D);
        // Right upper arm
        controllJoints[13].transform.rotation =
            Quaternion.FromToRotation(new Vector3(1, 0, 0), SynthesisJointsPos[13].Pos3D- SynthesisJointsPos[12].Pos3D);
        // Right elbow
        controllJoints[14].transform.rotation =
            Quaternion.FromToRotation(new Vector3(1, 0, 0), SynthesisJointsPos[14].Pos3D- SynthesisJointsPos[13].Pos3D);
    }

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
            yield return new WaitForSeconds(0.03f);
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
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftSideKick_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/walkCrossover_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/runSprint_EWMA.json";
        readInHumanPositionFile = "jsonPositionData/afterSynthesis/runSprintLinearMapping_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftFrontKick_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftFrontKick_stream_EWMA.json";
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
        //curPosition.x = curPosition.x + positionsIncreaseSpeed;
        //controllJoints[2].transform.localPosition = curPosition;

        if (isDirectApplySynthesisPositions)
        {
            controllJoints[6].transform.position = originHipPosition + correctOrigin;
            for (int i=0; i<6;++i)
                controllJoints[i].transform.position = SynthesisJointsPos[i].Pos3D + originHipPosition + correctOrigin;
            for(int i=6; i<15;++i)
                controllJoints[i+1].transform.position = SynthesisJointsPos[i].Pos3D + originHipPosition + correctOrigin;
        }

        // 計算並且更新joint的rotation
        // 旋轉後position又會再變動, 所以旋轉與position變動只能擇一apply到avatar身上
        if (isApplySynthesisRotations)
            computeRigJointsRotAndApply();
    }
}
