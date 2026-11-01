using UnityEngine;

// Reaches a character's hands to two markers while the animation plays: a driver's hands on the steering wheel, or a pedestrian's right hand on an
// umbrella's handle. A hand without a marker is left to the animation. It sits on the object that has the Animator, because Unity only calls
// OnAnimatorIK there, it needs "IK Pass" on the Animator Controller's layer (the setup tool turns that on), and it only works while this component is enabled.
[RequireComponent(typeof(Animator))]
public class HandIK : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null)
            return;

        Reach(AvatarIKGoal.LeftHand, leftHand);
        Reach(AvatarIKGoal.RightHand, rightHand);
    }

    private void Reach(AvatarIKGoal hand, Transform target)
    {
        if (target == null)
        {
            animator.SetIKPositionWeight(hand, 0f);
            return;
        }

        animator.SetIKPositionWeight(hand, 1f);
        animator.SetIKPosition(hand, target.position);
    }
}
