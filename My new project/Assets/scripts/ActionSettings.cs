using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ActionConfig", menuName ="Create Action Config", order =1)]
public class ActionSettings : ScriptableObject
{
    public positionApplyTest.ActionTypes actionType;
    public bool isApplyWristToRootPosition;
    public Vector3 wristRootPosScale;
    public Vector3 wristRootPosCorrection;
    public Vector3 wristRootPosLowerBound;
    public positionApplyTest.Rotation90 rotationOfAvatarInY;
}
