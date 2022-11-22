using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    public Animator myAnim;
    [Header("Right Hand")]
    [Range(0, 1)] public float rightHandleWeight;
    public Transform rightHand;
    public Transform rightHandHint;

    [Header("Lef Hand")]
    [Range(0, 1)] public float leftHandleWeight;
    public Transform leftHand;
    public Transform leftHandHint;

    private void OnAnimatorIK()
    {
        #region Right
        if (rightHand != null)
        {
            myAnim.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandleWeight);
            myAnim.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandleWeight);
            myAnim.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
            myAnim.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);
        }
        if (rightHandHint != null)
        {
            myAnim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
            myAnim.SetIKHintPosition(AvatarIKHint.RightElbow, rightHandHint.position);
        }
        #endregion

        #region Left
        if (leftHand != null)
        {
            myAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandleWeight);
            myAnim.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandleWeight);
            myAnim.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
            myAnim.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
        }
        if (leftHandHint != null)
        {
            myAnim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);
            myAnim.SetIKHintPosition(AvatarIKHint.LeftElbow, leftHandHint.position);
        }
        #endregion
    }
}
