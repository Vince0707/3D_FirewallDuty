using UnityEngine;

// Keeps a seated driver's hands on the steering wheel. The sitting animation has the hands out in front, as if on a keyboard; this reaches both hands
// to the two markers on the wheel (HandRest_L, HandRest_R) while the animation plays. It sits on the object that has the Animator, because Unity only
// calls OnAnimatorIK there, and it needs "IK Pass" on the Animator Controller's layer (the setup tool turns that on).
[RequireComponent(typeof(Animator))]
public class DriverHands : MonoBehaviour
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
