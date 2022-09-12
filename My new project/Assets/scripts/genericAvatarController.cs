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
    /// Generic avatar��joints game object
    /// ���Ǭ�: LeftUpperLeg, LeftLowerLeg, LeftFoot, 
                ///RightUpperLeg, RightLowerLeg, RightFoot,
                ///Hips, Spine, Chest, UpperChest,
                ///LeftUpperArm, LeftLowerArm, LeftHand,
                ///RightUpperArm, RightLowerArm, RightHand,
                ///Head
    /// </summary>
    public List<GameObject> jointsGO;
    public bool isApplyRotation;
    public bool isSetHipTo0;
    public bool isSetAvatarToTPose;

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
    /// ����human avatar joints��rotation�ƭ�
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
            recordTimeElapse += 0.03f;
            yield return new WaitForSeconds(0.03f);
        }
        jsonDeserializer jsonConverter = new jsonDeserializer();
        //jsonConverter.serializeAndOutputFile(new MediaPipeResult() { results = avatarRotationData.ToArray() }, "jsonRotationData/genericBodyDBRotation/leftSideKick.json");
        //jsonConverter.serializeAndOutputFile(new MediaPipeResult() { results = avatarRotationData.ToArray() }, "jsonRotationData/genericBodyDBRotation/leftSideKick_withHip.json");
        //jsonConverter.serializeAndOutputFile(new MediaPipeResult() { results = avatarRotationData.ToArray() }, "jsonRotationData/genericBodyDBRotation/runSprint0.5_withoutHip.json");
        //jsonConverter.serializeAndOutputFile(new MediaPipeResult() { results = avatarRotationData.ToArray() }, "jsonRotationData/genericBodyDBRotation/leftSideKick0.03_withHip.json");
        jsonConverter.serializeAndOutputFile(new MediaPipeResult() { results = avatarRotationData.ToArray() }, "jsonRotationData/genericBodyDBRotation/runSprint0.03_withHip.json");
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
            //"jsonPositionData/bodyMotionPosition/leftFrontKickPositionFullJointsWithHead.json"  // ��X�Ҧ�����joints�ϥΪ��ɦW
            //"jsonPositionData/bodyMotionPosition/genericAvatar/leftSideKickPositionFullJointsWithHead_withoutHip.json"  // ��X�Ҧ�����joints�ϥΪ��ɦW
            "jsonPositionData/bodyMotionPosition/genericAvatar/leftSideKickPositionFullJointsWithHead_withHip.json"  // ��X�Ҧ�����joints�ϥΪ��ɦW
            //"jsonPositionData/bodyMotionPosition/genericAvatar/runSprintPositionFullJointsWithHead0.5_withoutHip.json"  // ��X�Ҧ�����joints�ϥΪ��ɦW
            //"jsonPositionData/bodyMotionPosition/genericAvatar/runSprintPositionFullJointsWithHead_withoutHip.json"  // ��X�Ҧ�����joints�ϥΪ��ɦW
            //"jsonPositionData/bodyMotionPosition/genericAvatar/runSprintPositionFullJointsWithHead_withHip.json"  // ��X�Ҧ�����joints�ϥΪ��ɦW
            //"jsonPositionData/bodyMotionPosition/TPose.json" // ��XT-pose��position
            //"jsonPositionData/bodyMotionPosition/genericAvatar/TPose.json" // ��XT-pose��position
            );
        yield return null;
    }
    
    /// <summary>
    /// �Ȧsposition��ƨ��ܼƷ�, ���O�S���x�s���ɮ�
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
    /// �N�Ȧs�b*�ܼƷ���position����x�s���ɮ�
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
    /// ��J�h��Ū���ɮ׸��|, �N�����ഫ���x�s�ɮצ�m���|
    /// </summary>
    /// <param name="fileNames"></param>
    public void changeCurRecordPosFileNM(List<string> fileNames)
    {
        HumanRotationFileNMs = new List<string>();
        foreach (string str in fileNames)
        {
            // ��s�ɮצW��
            string fileName = Path.GetFileName(str);
            // �����ɮצW�� + "_position" + ".json"
            // ���wfolder�x�sposition����X���
            HumanRotationFileNMs.Add(
                //Path.Combine(Application.dataPath, "jsonPositionData/leftSideKickCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/leftSideKickLinearMappingCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/walkCrossoverCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/walkInjuredCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/runSprintCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/runSprintLinearMappingCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/runSprintCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/test/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/leftSideKickLinearMappingCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/leftSideKickStreamLinearMappingCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/leftFrontKickStreamLinearMappingCombinations/" + fileName)
                Path.Combine(Application.dataPath, "jsonPositionData/leftFrontKickStreamLinearMapping/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/leftFrontKickLinearMappingCombinations/" + fileName)
                //Path.Combine(Application.dataPath, "jsonPositionData/walkLinearMappingCombinations/" + fileName)
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

    /// <summary>
    /// �o�Ө�ƨS���@��, �]��Hip��rotation, position, ...��T�S����k������ק�, 
    /// Hip����T�û����|�Qanimation/animator����T���л\(�N����LateUpdata����]�@��)
    /// �Q�n�NHip rotation�û��k0, �u��z�L�ק��lanimation�������(�ثe�ĥγo�ذ��k)
    /// </summary>
    private void setHipRotTo0()
    {
        jointsGO[6].transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// �Navatar���ʧ@��l�Ʀ�T pose�����A, 
    /// ���P�N�Ҧ����n��joints��local rotation�]��0
    /// �ݭn�`�N, �Y�O�Ʊ�Hip rotation��0, �h�n�D��쥻Hip�N�O 0��animation, 
    /// �Ω����avatar T pose��T, ����python rotation to position�p��ϥ�
    /// </summary>
    private void initAvatarToTPose()
    {
        for(int i=0; i<jointsGO.Count;++i)
        {
            jointsGO[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
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
        if (isSetAvatarToTPose)
            initAvatarToTPose();
    }
}
