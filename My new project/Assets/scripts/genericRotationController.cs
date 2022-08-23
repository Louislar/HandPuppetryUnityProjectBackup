using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class genericRotationController : MonoBehaviour
{
    public genericAvatarController genericAvatarController;
    private jsonDeserializer jsonDeserializer;
    private MediaPipeResult rotationResult;
    private handLMsController.JointBone[] jointBones;

    public bool updateRotationToAvatar;

    // ���s�h��rotation applied avatar positions
    public bool isReadMultipleResultsFileNames;
    public bool isSaveMultipleResultsFile;
    private List<string> multipleRotResultsFileNames;
    private List<MediaPipeResult> mappedRotResults;
    public string curVisualFileName;

    /// <summary>
    /// ��s��������ਤ�ר�avatar���W
    /// </summary>
    public void updateBodyJointRotation()
    {
        genericAvatarController.updateRotation(
            jointBones[1].flexionRotation, jointBones[3].flexionRotation,
            jointBones[0].flexionRotation, jointBones[0].abductionRotation,
            jointBones[2].flexionRotation, jointBones[2].abductionRotation
            );
    }

    /// <summary>
    /// ��sjoint���ਤ�סA�ϥ�Ū�������׸�T
    /// </summary>
    /// <param name="newRots"></param>
    public void updateJointRotations(MediaPipeHandLMs newRots)
    {
        for (int i = 0; i < newRots.data.Count; ++i)
        {
            jointBones[i].flexionRotation = newRots.data[i].x;
            jointBones[i].abductionRotation = newRots.data[i].z;
        }

    }

    IEnumerator updateRotationOnce()
    {
        int curIndex = 0;
        while (curIndex < rotationResult.results.Length)
        {
            updateJointRotations(rotationResult.results[curIndex]);
            ++curIndex;
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }

    /// <summary>
    /// �̧�apply/����h��mapped rotations���W�A
    /// �Ω�ݭnrecord�h��mapping function��position���G�A
    /// �קK�ݭn���s�h��mapping functions�ɻݭn���ƶ}�l�P����
    /// �s�W: ����}�l���s��ƥH�α����x�s���s���G���
    /// singlePositionsRecorder() and saveRecordPos()
    /// TODO: ���շs�W"����}�l���s��ƥH�α����x�s���s���G���"�����G�p��
    /// </summary>
    /// <returns></returns>
    IEnumerator updateMultipleJointsRotationsSequential()
    {
        for (int i = 0; i < mappedRotResults.Count; ++i)
        {

            curVisualFileName = multipleRotResultsFileNames[i].Substring(
                multipleRotResultsFileNames[i].Length - 50, 50
                ); //��e�����ɮצW��
            MediaPipeResult curRotationMappedResult = mappedRotResults[i];
            int curIndex = 0;
            Coroutine tmpCo = null;
            if (isSaveMultipleResultsFile)
                tmpCo = StartCoroutine(genericAvatarController.singlePositionsRecorder());
            while (curIndex < curRotationMappedResult.results.Length)
            {
                updateJointRotations(curRotationMappedResult.results[curIndex]);
                ++curIndex;
                yield return new WaitForSeconds(0.03f);
            }
            if (isSaveMultipleResultsFile)
            {
                StopCoroutine(tmpCo);
                genericAvatarController.saveRecordPos(i);
            }
        }
        yield return null;
    }


    /// <summary>
    /// ����True�PFalse�ƦC�զX��string�ǦC
    /// e.g. True, False, True
    /// </summary>
    /// <param name="size">���ͪ��ǦC����</param>
    /// <returns></returns>
    public List<string> boolPermutation(int size = 1)
    {
        //float totalCount = Mathf.Pow(2, size);
        List<string> permutationList = new List<string>();
        for (int i = 0; i < size; ++i)
        {
            if (permutationList.Count == 0)
            {
                permutationList.Add("True, ");
                permutationList.Add("False, ");
                continue;
            }
            int curLength = permutationList.Count;
            List<string> newPermutationList = new List<string>();
            for (int j = 0; j < curLength; ++j)
            {
                newPermutationList.Add(permutationList[j] + "True, ");
                newPermutationList.Add(permutationList[j] + "False, ");
            }
            permutationList = newPermutationList;
        }
        // �����̫᪺", "�A�åB�[�W�A��
        for (int i = 0; i < permutationList.Count; ++i) permutationList[i] = permutationList[i].Substring(0, permutationList[i].Length - 2);
        for (int i = 0; i < permutationList.Count; ++i) permutationList[i] = "(" + permutationList[i] + ")";
        //foreach (string str in permutationList) print(str);
        return permutationList;
    }

    // Start is called before the first frame update
    void Start()
    {
        jointBones = new handLMsController.JointBone[4];
        for (var i = 0; i < 4; i++) jointBones[i] = new handLMsController.JointBone();
        mappedRotResults = new List<MediaPipeResult>();
        jsonDeserializer = new jsonDeserializer();

        // Read multiple mapping rotation results
        List<string> boolPermutationStrings = boolPermutation(6);
        //string rootFileName = "jsonRotationData/handRotationAfterMapping/leftFrontKickCombinations/leftFrontKick";
        //string rootFileName = "jsonRotationData/handRotationAfterMapping/leftFrontKickStreamLinearMappingCombinations/leftFrontKick";
        //string rootFileName = "jsonRotationData/handRotationAfterMapping/leftFrontKickLinearMappingCombinations/leftFrontKick";
        //string rootFileName = "jsonRotationData/handRotationAfterMapping/leftSideKickCombinations/leftSideKick";
        //string rootFileName = "jsonRotationData/handRotationAfterMapping/leftSideKickLinearMappingCombinations/leftSideKick";
        //string rootFileName = "jsonRotationData/handRotationAfterMapping/leftSideKickStreamLinearMappingCombinations/leftSideKick";
        //string rootFileName = "jsonRotationData/handRotationAfterMapping/walkCrossoverCombinations/walkCrossover";
        //string rootFileName = "jsonRotationData/handRotationAfterMapping/walkInjuredCombinations/walkInjured";
        //string rootFileName = "jsonRotationData/handRotationAfterMapping/runSprintCombinations/runSprint";
        //string rootFileName = "jsonRotationData/handRotationAfterMapping/runSprintLinearMappingCombinations/runSprint";
        //string rootFileName = "jsonRotationData/handRotationAfterMapping/runSprintStreamLinearMapping2Combinations/runSprint";
        //string rootFileName = "jsonRotationData/handRotationAfterMapping/runSprintStreamLinearMappingCombinations/runSprint";
        string rootFileName = "jsonRotationData/handRotationAfterMapping/walkLinearMapping/walk";
        // generate�Ҧ�true false�զX���ɦW�A�̫�Ҧ��ɦW�A�ɤW".json"
        multipleRotResultsFileNames = new List<string>();
        foreach (string str in boolPermutationStrings) multipleRotResultsFileNames.Add(rootFileName + str + ".json");
        // ���եΡA���ϥΤp�ƶq����ƴ���
        multipleRotResultsFileNames = new List<string>
        {
            //"jsonRotationData/handRotationRecord/leftFrontKickStream.json", 
            //"jsonRotationData/handRotationAfterMapping/leftFrontKickCombinations/leftFrontKick(True, False, True, False, False, False).json", 
            //"jsonRotationData/handRotationAfterMapping/leftFrontKickLinearMappingCombinations/leftFrontKick(True, True, True, True, True, True).json",
            //"jsonRotationData/handRotationAfterMapping/newLeftFrontKickCombinations/leftFrontKick(True, False, True, False, False, False).json"
            //"jsonRotationData/handRotationAfterMapping/leftSideKickCombinations/leftSideKick(False, True, False, False, False, False).json",
            //"jsonRotationData/handRotationAfterMapping/leftSideKickCombinations/leftSideKick(True, True, True, False, False, False).json",
            //"jsonRotationData/handRotationAfterMapping/leftSideKickLinearMappingCombinations/leftSideKick(False, True, True, False, False, False).json",
            //"jsonRotationData/handRotationAfterMapping/leftSideKickLinearMappingCombinations/leftSideKick(True, True, True, False, False, False).json",
            //"jsonRotationData/handRotationAfterMapping/leftSideKickLinearMappingCombinations/leftSideKick(True, True, True, True, True, True).json",
            //"jsonRotationData/handRotationAfterMapping/runSprintStreamLinearMappingCombinations/runSprint(True, True, True, True, True, True).json",
            //"jsonRotationData/handRotationAfterMapping/runSprintLinearMappingCombinations/runSprint(True, True, True, True, True, True).json",
            //"jsonRotationData/handRotationAfterMapping/runSprintLinearMappingCombinations/runSprint(True, False, True, True, False, True).json"
            //"jsonRotationData/handRotationAfterMapping/runSprintCombinations/runSprint(True, True, True, True, True, True).json",
            //"jsonRotationData/handRotationAfterMapping/runSprintCombinations/runSprint(True, False, True, True, False, True).json"
            "jsonRotationData/handRotationAfterMapping/walkLinearMapping/walk(True, True, True, True, True, True).json"

        };
        // ���ե� end
        foreach (string str in multipleRotResultsFileNames)
        {
            mappedRotResults.Add(jsonDeserializer.readAndParseRotation(str));
        }

        if (isReadMultipleResultsFileNames)
        {
            // �I�sfunction�A���w�ݭnavatarController�������Ҧ��ɮצW��
            genericAvatarController.changeCurRecordPosFileNM(multipleRotResultsFileNames);
            StartCoroutine(updateMultipleJointsRotationsSequential());
        }
        else
        {
            // Update single mapped rotation result
            //StartCoroutine(updateRotationOnce());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (updateRotationToAvatar)
        {
            updateBodyJointRotation();
        }
    }
}
