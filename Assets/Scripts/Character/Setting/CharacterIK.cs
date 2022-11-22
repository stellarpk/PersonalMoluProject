using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIK : MonoBehaviour
{
    public Animator myAnim;
    public Transform gunPivot;
    public Transform leftHandMount; // 총의 왼쪽 손잡이, 왼손이 위치할 지점
    public Transform rightHandMount; // 총의 오른쪽 손잡이, 오른손이 위치할 지점

    private void OnAnimatorIK(int layerIndex)
    {
        gunPivot.position =
            myAnim.GetIKHintPosition(AvatarIKHint.RightElbow);
        Debug.Log($"{AvatarIKHint.RightElbow}");
        // IK를 사용하여 왼손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        myAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        myAnim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        myAnim.SetIKPosition(AvatarIKGoal.LeftHand,
            leftHandMount.position);
        myAnim.SetIKRotation(AvatarIKGoal.LeftHand,
            leftHandMount.rotation);

        // IK를 사용하여 오른손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        myAnim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        myAnim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        myAnim.SetIKPosition(AvatarIKGoal.RightHand,
            rightHandMount.position);
        myAnim.SetIKRotation(AvatarIKGoal.RightHand,
            rightHandMount.rotation);
    }
}
