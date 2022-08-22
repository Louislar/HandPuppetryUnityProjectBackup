using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class genericAvatarController : MonoBehaviour
{
    // TODOs (generic animation)
    // 1. avatar position recorder [Done]
    // 2. avatar rotation recorder [Done]
    // 3. avatar rotation apply [Done]
    // 4. avatar position apply(already implemented)

    /// <summary>
    /// Generic avatar的joints game object
    /// 順序為: LeftUpperLeg, LeftLowerLeg, LeftFoot, 
                ///RightUpperLeg, RightLowerLeg, RightFoot,
                ///Hips, Spine, Chest, UpperChest,
                ///LeftUpperArm, LeftLowerArm, LeftHand,
                ///RightUpperArm, RightLowerArm, RightHand,
                ///Head
    /// </summary>
    public List<GameObject> jointsGO;
    public bool isApplyRotation;
    public bool isSetHipTo0;

    [Header("Rotation recording settings")]
    public bool isRecordHumanRotation;
    public float recordLength;
    private List<MediaPipeHandLMs> avatarRotationData;

    [Header("Position recording settings")]
    public bool isRecordHumanPosition;
    public float positionRecordLength;
    private List<MediaPipeHandLMs> avatarPositionData;

    [Header("Multi Rotation apply and position recording settings")]
    public List<string> HumanRotationFileNMs;
    private handLMsController.JointBone[] jointBones;

    /// <summary>
    /// 紀錄human avatar joints的rotation數值
    /// </summary>
    /// <returns></returns>
    public IEnumerator rotationRecorder()
    {
        float recordTimeElapse = 0;
        List<int> recoredJoints = new List<int>()
        {
            0, 1, 3, 4
        };
        avatarRotationData = new List<MediaPipeHandLMs>();
        while (recordTimeElapse < recordLength)
        {
            List<LMDataPoint> tmpDataPonts = new List<LMDataPoint>();
            foreach (int _aBonIdx in recoredJoints)
            {
                tmpDataPonts.Add(new LMDataPoint()
                {
                    x = jointsGO[_aBonIdx].transform.localEulerAngles.x,
                    y = jointsGO[_aBonIdx].transform.localEulerAngles.y,
                    z = jointsGO[_aBonIdx].transform.localEulerAngles.z
                });
            }
            avatarRotationData.Add(new MediaPipeHandLMs()
            {
                time = recordTimeElapse,
                data = tmpDataPonts
            });
            recordTimeElapse += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        jsonDeserializer jsonConverter = new jsonDeserializer();
        //jsonConverter.serializeAndOutputFile(new MediaPipeResult() { results = avatarRotationData.ToArray() }, "jsonRotationData/genericBodyDBRotation/leftSideKick.json");
        jsonConverter.serializeAndOutputFile(new MediaPipeResult() { results = avatarRotationData.ToArray() }, "jsonRotationData/genericBodyDBRotation/leftSideKick_withHip.json");
        yield return null;
    }

    public IEnumerator positionRecorder()
    {
        float recordTimeElapse = 0;
        List<int> recoredJoints = new List<int>()
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 
            11, 12, 13, 14, 15, 16
        };
        avatarPositionData = new List<MediaPipeHandLMs>();
        while (recordTimeElapse < positionRecordLength)
        {
            List<LMDataPoint> tmpDataPonts = new List<LMDataPoint>();
            foreach (int _aBoneIdx in recoredJoints)
            {
                tmpDataPonts.Add(new LMDataPoint()
                {
                    x = jointsGO[_aBoneIdx].transform.position.x,
                    y = jointsGO[_aBoneIdx].transform.position.y,
                    z = jointsGO[_aBoneIdx].transform.position.z
                });
            }
            avatarPositionData.Add(new MediaPipeHandLMs()
            {
                time = recordTimeElapse,
                data = tmpDataPonts
            });
            recordTimeElapse += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        jsonDeserializer jsonConverter = new jsonDeserializer();
        jsonConverter.serializeAndOutputFile(
            new MediaPipeResult() { results = avatarPositionData.ToArray() },
            //"jsonPositionData/bodyMotionPosition/leftFrontKickPosition.json"
            //"jsonPositionData/bodyMotionPosition/leftFrontKickPositionFullJointsWithHead.json"  // 輸出所有身體joints使用的檔名
            //"jsonPositionData/bodyMotionPosition/genericAvatar/leftSideKickPositionFullJointsWithHead.json"  // 輸出所有身體joints使用的檔名
            "jsonPositionData/bodyMotionPosition/genericAvatar/leftSideKickPositionFullJointsWithHead_withHip.json"  // 輸出所有身體joints使用的檔名
                                                                                                             //"jsonPositionData/bodyMotionPosition/TPose.json" // 輸出T-pose的position
            );
        yield return null;
    }
    
    /// <summary>
    /// 暫存position資料到變數當中, 但是沒有儲存到檔案
    /// </summary>
    /// <returns></returns>
    public IEnumerator singlePositionsRecorder()
    {
        print("Start to record single positions. ");
        List<int> recoredJoints = new List<int>()
        {
            0, 1, 2, 3, 4, 5, 6
        };
        // initialize the data array
        avatarPositionData = new List<MediaPipeHandLMs>();
        float recordTimeElapse = 0;
        // start recording positions
        while (true)
        {
            List<LMDataPoint> tmpDataPonts = new List<LMDataPoint>();
            foreach (int _aBoneIdx in recoredJoints)
            {
                tmpDataPonts.Add(new LMDataPoint()
                {
                    x = jointsGO[_aBoneIdx].transform.position.x,
                    y = jointsGO[_aBoneIdx].transform.position.y,
                    z = jointsGO[_aBoneIdx].transform.position.z
                });
            }
            avatarPositionData.Add(new MediaPipeHandLMs()
            {
                time = recordTimeElapse,
                data = tmpDataPonts
            });
            recordTimeElapse += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        yield return null;
    }

    /// <summary>
    /// 將暫存在*變數當中的position資料儲存到檔案
    /// *avatarPositionData
    /// </summary>
    /// <param name="curFileIdx"></param>
    public void saveRecordPos(int curFileIdx)
    {
        jsonDeserializer jsonConverter = new jsonDeserializer();
        jsonConverter.serializeAndOutputFile(
            new MediaPipeResult() { results = avatarPositionData.ToArray() },
            HumanRotationFileNMs[curFileIdx]
            );
    }

    /// <summary>
    /// 輸入多組讀取檔案路徑, 將它們轉換成儲存檔案位置路徑
    /// </summary>
    /// <param name="fileNames"></param>
    public void changeCurRecordPosFileNM(List<string> fileNames)
    {
        HumanRotationFileNMs = new List<string>();
        foreach (string str in fileNames)
        {
            // 更新檔案名稱
            string fileName = Path.GetFileName(str);
            // 改變檔案名稱 + "_position" + ".json"
            // 指定folder儲存position的輸出資料
            HumanRotationFileNMs.Add(
                //Path.Combine(Application.dataPath, "jsonPositionData/leftSideKickCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/leftSideKickLinearMappingCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/walkCrossoverCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/walkInjuredCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/runSprintCombinations/" + fileName)
                Path.Combine(Application.dataPath, "jsonPositionData/runSprintLinearMappingCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/runSprintCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/test/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/leftSideKickLinearMappingCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/leftSideKickStreamLinearMappingCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/leftFrontKickStreamLinearMappingCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/leftFrontKickLinearMappingCombinations/" + fileName)
                );
            // print(curHumanRotationFileNM);
        }
    }

    public void updateRotation(float leftKneeRotation, float rightKneeRotation, 
        float leftUpperLeg1, float leftUpperLeg2, 
        float rightUpperLeg1, float rightUpperLeg2)
    {
        jointBones[1].rotation = Quaternion.Euler(leftKneeRotation, 0.0f, 0.0f);
        jointBones[3].rotation = Quaternion.Euler(rightKneeRotation, 0.0f, 0.0f);

        jointBones[0].rotation = Quaternion.Euler(leftUpperLeg1, 0.0f, leftUpperLeg2);
        jointBones[2].rotation = Quaternion.Euler(rightUpperLeg1, 0.0f, rightUpperLeg2);
    }

    private void rotationApplyToAvatar()
    {
        jointsGO[1].transform.localRotation = jointBones[1].rotation;
        jointsGO[4].transform.localRotation = jointBones[3].rotation;

        jointsGO[0].transform.localRotation = jointBones[0].rotation;
        jointsGO[3].transform.localRotation = jointBones[2].rotation;
    }

    private void setHipRotTo0()
    {
        jointsGO[6].transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        jointBones = new handLMsController.JointBone[4];
        for (var i = 0; i < 4; i++) jointBones[i] = new handLMsController.JointBone();

        if (isRecordHumanRotation)
        {
            StartCoroutine(rotationRecorder());
        }
        if(isRecordHumanPosition)
        {
            StartCoroutine(positionRecorder());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (isSetHipTo0)
            setHipRotTo0();
        if (isApplyRotation)
            rotationApplyToAvatar();
    }
}
