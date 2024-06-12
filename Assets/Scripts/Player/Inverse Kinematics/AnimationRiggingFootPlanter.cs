using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimationRiggingFootPlanter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MultiParentConstraint footRefConstraint;
    [SerializeField] private TwoBoneIKConstraint footIK;
    [SerializeField] private Transform iKTarget;
    [Header("Values")]
    [SerializeField] private float rayYOffset = 1;
    [SerializeField] private float rayDistance = 0.1f;
    [SerializeField] private float plantedYOffset = 0.1f;
    [SerializeField] private LayerMask mask;

    private Vector3 rayOrigin;

    private void LateUpdate()
    {
        //The constraints work after the animation transform system but before the late update in scripts?? So there is an extra parent here before the raycast so I can figure out where to ref

        footIK.weight = 0; //Setting IK weight to 0 to prevent movement
        //footRefConstraint.weight = 1; //Setting IK weight to 1 to allow the footRef to move
        transform.position = footRefConstraint.transform.position; //Transform to footRefConstraint
        rayOrigin = transform.position + Vector3.up * rayYOffset; //Set how high you want the ray to be
        var footPos = footRefConstraint.transform.position;

        if (Physics.Raycast(rayOrigin, Vector3.down, out var hit, rayDistance, mask))
        {
            var hitPosY = hit.point.y + plantedYOffset;
            if (footPos.y < hitPosY)
            {
                footIK.weight = 1; //Moving the foot using the ray
                var pos = hit.point;
                pos.y += plantedYOffset;
                iKTarget.position = pos; //Set the inverse kinematics target to the target position
                var tarRot = Quaternion.FromToRotation(Vector3.up, hit.normal) * footRefConstraint.transform.rotation; //Rotating the foot using the ray
                iKTarget.rotation = tarRot; //Set the inverse kinematics rotation to the target rotation
            }
        }
        Debug.DrawRay(rayOrigin, Vector3.down * rayDistance, Color.red); //Simple debug function
    }
}
