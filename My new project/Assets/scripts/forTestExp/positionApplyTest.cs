using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class positionApplyTest : MonoBehaviour
{
    public enum Rotation90
    {
        front = 0, 
        faceLeft = 90, 
        faceRight = -90, 
        back = 180
    }

    public enum ActionTypes
    {
        frontKick,
        sideKick,
        runSprint,
        jumpJoy,
        twoLegJump
    }

    public List<GameObject> controllJoints; // Fill this list in the Unity inspector
    public List<GameObject> rigHintJoints;
    public Vector3 curPosition;
    public bool applyRotationOfAvatarAlongY;
    public Rotation90 rotationOfAvatarInY;
    public Transform avatarRootOB;

    [Header("Position read/apply in settings")]
    public bool isReadSynthesisPositions;
    public bool isDirectApplySynthesisPositions;
    public bool isApplySynthesisRotations;
    public bool isApplyToRigJoints;


    [Header("Wrist position read/apply in settings")]
    public bool isApplyWristToRootPosition;
    // Suitable scale (-10, -5, 1)
    public Vector3 wristRootPosScale;
    // �ե�hip postition��m, �����brun time�X�{�b�ڷQ�n����m 
    // default�O (0, 0, 0) 
    public Vector3 wristRootPosCorrection;
    // hip position���̤p�ƭ�
    // default�O�o��gameobject��position (2, 0, 2) 
    public Vector3 wristRootPosLowerBound;
    public string readInHumanPositionFile;
    public Vector3 originHipPosition;
    public Vector3 correctOrigin;
    public int readPositionsJointsNum;
    private MediaPipeResult readInHumanPositionResult;
    private handLMsController.JointBone[] SynthesisJointsPos;
    private Vector3 originalRootPos;

    [Header("Action settings")]
    public UIController uiController;
    public ActionTypes actionType;
    public List<ActionSettings> actionSettings;

    [Header("Keyboard control settings")]
    public Vector2 keyBoardControlScale;    // Set (0,0) indicates that no effect 

    /// <summary>
    /// �ھ�action settings�ɮ�, ���ܥثe���]�w�� 
    /// </summary>
    public void applyActionSettings(ActionTypes actionTypeIn)
    {
        List<ActionSettings> _actionSetting = actionSettings.Where(p => p.actionType == actionTypeIn).ToList();
        if (_actionSetting.Count != 0)
        {
            isApplyWristToRootPosition = _actionSetting[0].isApplyWristToRootPosition;
            wristRootPosScale = _actionSetting[0].wristRootPosScale;
            wristRootPosCorrection = _actionSetting[0].wristRootPosCorrection;
            wristRootPosLowerBound = _actionSetting[0].wristRootPosLowerBound;
            rotationOfAvatarInY = _actionSetting[0].rotationOfAvatarInY;
        }
        else
            print(actionTypeIn + " 's setting not found!");
    }

    /// <summary>
    /// ���Jaction���]�w�ɮ�, 
    /// </summary>
    public void loadActionSettings()
    {

    }

    /// <summary>
    /// �ק��e�ϥΪ�action����
    /// </summary>
    /// <param name="actionTypeIn"></param>
    public void setActionType(ActionTypes actionTypeIn)
    {
        actionType = actionTypeIn;
    }

    /// <summary>
    /// �z�L�ק�hip rotation�F�������avatar���ت�
    /// </summary>
    /// <param name="rot"></param>
    public void rotateAvatarAlongY(Rotation90 rot)
    {
        //this.transform.rotation = Quaternion.Euler(0, (int) rot, 0);
        //controllJoints[6].transform.Rotate(new Vector3(0, (int)rot, 0));
        controllJoints[6].transform.Rotate(0, (int)rot, 0, Space.World);

        //avatarRootOB.rotation = Quaternion.Euler(0, (int)rot, 0);
    }

    /// <summary>
    /// �Q�Φ��p���ⳡlandmarks���avatar��root position (�`�N, ���Ohip position) 
    /// �ϥ�joint of wrist ��savatar��root position 
    /// �n�t�Xscale�Y���Y��wrist��position�ƭ�, �]�������ƭȽd��O[0, 1] 
    /// </summary>
    /// <param name="handLandmarks"></param>
    public void updateRootPositionOnce(MediaPipeHandLMs handLandmarks)
    {
        if (isApplyWristToRootPosition)
        {
            // �p�Gposition�p��lower bound�h�ϥ�lower bound�N�� 
            Vector3 tmpVec = new Vector3(
                originalRootPos.x + handLandmarks.data[0].x * wristRootPosScale.x + wristRootPosCorrection.x,
                originalRootPos.y + handLandmarks.data[0].y * wristRootPosScale.y + wristRootPosCorrection.y,
                this.transform.localPosition.z
                );

            this.transform.localPosition = new Vector3(
                tmpVec.x > wristRootPosLowerBound.x ? tmpVec.x : wristRootPosLowerBound.x,
                tmpVec.y > wristRootPosLowerBound.y ? tmpVec.y : wristRootPosLowerBound.y,
                tmpVec.z
                );
        }
    }

    /// <summary>
    /// �p��U��joints��rotation, �z�פW�u�ݭn�p������P���}���U���joint������Y�i
    ///         �ثe�p�⵲�Gapply��avatar���W�A�٬O�P����apply position���@�Ǹ��t
    /// </summary>
    public void computeRigJointsRotAndApply()
    {
        Debug.DrawRay(originHipPosition + correctOrigin, SynthesisJointsPos[0].Pos3D, Color.green);
        // Hip
        controllJoints[6].transform.rotation =
            Quaternion.FromToRotation(new Vector3(-0.08207779f, -0.06751716f, -0.01599556f), SynthesisJointsPos[0].Pos3D);
        //Quaternion hipGlobalRot = Quaternion.FromToRotation(new Vector3(0, 1, 0), SynthesisJointsPos[7].Pos3D - SynthesisJointsPos[6].Pos3D);
        //controllJoints[6].transform.localRotation = hipGlobalRot;
        // Spine 
        //controllJoints[7].transform.rotation =
        //    Quaternion.FromToRotation(new Vector3(0, 1, 0), SynthesisJointsPos[8].Pos3D - SynthesisJointsPos[7].Pos3D);
        // Chest 
        //controllJoints[8].transform.rotation =
        //    Quaternion.FromToRotation(new Vector3(0, 1, 0), SynthesisJointsPos[9].Pos3D - SynthesisJointsPos[8].Pos3D);
        // Left upper leg
        //Quaternion upperLegGlobalRot = Quaternion.FromToRotation(new Vector3(0, -1, 0), SynthesisJointsPos[1].Pos3D - SynthesisJointsPos[0].Pos3D);
        //controllJoints[0].transform.localRotation =  upperLegGlobalRot * controllJoints[6].transform.rotation;
        controllJoints[0].transform.rotation =
            Quaternion.FromToRotation(new Vector3(0, -1, 0), SynthesisJointsPos[1].Pos3D - SynthesisJointsPos[0].Pos3D);
        // Left knee
        //Quaternion lowerLegGlobalRot = Quaternion.FromToRotation(new Vector3(0, -1, 0), SynthesisJointsPos[2].Pos3D - SynthesisJointsPos[1].Pos3D);
        //controllJoints[1].transform.localRotation = controllJoints[6].transform.rotation * Quaternion.Inverse(controllJoints[0].transform.localRotation)  * lowerLegGlobalRot;
        controllJoints[1].transform.rotation =
            Quaternion.FromToRotation(new Vector3(0, -1, 0), SynthesisJointsPos[2].Pos3D - SynthesisJointsPos[1].Pos3D);
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
    /// �NŪ����positions apply��animation rigging��target, hint joints
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
    /// ��s�@��synthesis�᪺position data
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
    /// ��ssynthesis�����᪺position data��avatar�W
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
        uiController.buttonListener += (typeStr) => {
            ActionTypes _actionType;
            Enum.TryParse(typeStr, out _actionType);
            setActionType(_actionType);
            applyActionSettings(_actionType);
        };

        //avatarRootOB.rotation *= Quaternion.Euler(0, (int)rotationOfAvatarInY, 0);

        curPosition = new Vector3();
        curPosition = controllJoints[2].transform.localPosition;

        originalRootPos = new Vector3();
        originalRootPos = transform.localPosition;

        // init positions array
        SynthesisJointsPos = new handLMsController.JointBone[readPositionsJointsNum];
        for (int i = 0; i < readPositionsJointsNum; ++i) { SynthesisJointsPos[i] = new handLMsController.JointBone(); }
        // Apply synthesis positions to avatar
        jsonDeserializer jsonDeserializer = new jsonDeserializer();
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftSideKick_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/walkCrossover_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/runSprint_TFTTFT_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/runSprintLinearMapping_TFTTFT_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/runSprintLinearMapping_generic_TTTTTT_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/runSprintLinearMapping_generic_allLeft_TTTTTT_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/runSprintStreamLinearMapping_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/runSprintStreamLinearMapping_TFTTFT_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftSideKick_TTTFFF_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftSideKick_generic_TTTFFF_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftSideKickLinearMapping_generic_TTTFFF_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftSideKickLinearMapping_TTTTTT_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftSideKickLinearMapping_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftSideKickStreamLinearMapping_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftSideKickStreamLinearMapping_FTTFFF_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftFrontKick_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftFrontKick_stream_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftFrontKickStreamLinearMapping_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftFrontKickStreamLinearMapping_TFFTTT_EWMA.json";
        readInHumanPositionFile = "jsonPositionData/afterSynthesis/leftFrontKickStreamLinearMapping_TFFTTT_075_EWMA.json";
        //readInHumanPositionFile = "jsonPositionData/afterSynthesis/walkInjuredStreamLinearMapping_TFTTFT_EWMA.json";
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
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.transform.Translate(new Vector3(0, 0, -keyBoardControlScale.y));
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            this.transform.Translate(new Vector3(0, 0, keyBoardControlScale.y));
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Translate(new Vector3(keyBoardControlScale.y, 0, 0));
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Translate(new Vector3(-keyBoardControlScale.y, 0, 0));
        }
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

        // �p��åB��sjoint��rotation
        // �����position�S�|�A�ܰ�, �ҥH����Pposition�ܰʥu��ܤ@apply��avatar���W
        if (isApplySynthesisRotations)
            computeRigJointsRotAndApply();
        if (applyRotationOfAvatarAlongY)
            rotateAvatarAlongY(rotationOfAvatarInY);

    }
}
