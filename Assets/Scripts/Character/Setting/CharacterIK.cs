using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIK : MonoBehaviour
{
    public Animator myAnim;
    public Transform gunPivot;
    public Transform leftHandMount; // ���� ���� ������, �޼��� ��ġ�� ����
    public Transform rightHandMount; // ���� ������ ������, �������� ��ġ�� ����

    private void OnAnimatorIK(int layerIndex)
    {
        gunPivot.position =
            myAnim.GetIKHintPosition(AvatarIKHint.RightElbow);
        Debug.Log($"{AvatarIKHint.RightElbow}");
        // IK�� ����Ͽ� �޼��� ��ġ�� ȸ���� ���� ������ �����̿� �����
        myAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        myAnim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        myAnim.SetIKPosition(AvatarIKGoal.LeftHand,
            leftHandMount.position);
        myAnim.SetIKRotation(AvatarIKGoal.LeftHand,
            leftHandMount.rotation);

        // IK�� ����Ͽ� �������� ��ġ�� ȸ���� ���� ������ �����̿� �����
        myAnim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        myAnim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        myAnim.SetIKPosition(AvatarIKGoal.RightHand,
            rightHandMount.position);
        myAnim.SetIKRotation(AvatarIKGoal.RightHand,
            rightHandMount.rotation);
    }
}
