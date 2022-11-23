using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIK : MonoBehaviour
{
    public Animator myAnim;
    public Transform gunPivot;
    public Transform leftHandMount;
    [Range(0, 1)] public float weight;
    private void OnAnimatorIK(int layerIndex)
    {
        if (leftHandMount != null)
        {
            myAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
            myAnim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandMount.position);
            myAnim.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
            myAnim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandMount.rotation);
        }

    }
}
