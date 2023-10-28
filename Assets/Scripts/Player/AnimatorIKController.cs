using Mirror;
using UnityEngine;
public class AnimatorIKController : NetworkBehaviour
{
    public PlayerMovementController playerMovementController;
    //syncvarIK Animations
    [SyncVar] private float weight = 0.6f;
    [SyncVar] private float bodyWeight = 0.2f;
    [SyncVar] private float headWeight = 1.2f;
    [SyncVar] private float distance = 25;
    [SyncVar] private Ray lookAtRay = new Ray();

    private void OnAnimatorIK()
    {
        if (!playerMovementController.GetHasAnimator()) return;
        if (NetworkClient.ready)
        {
            if (isLocalPlayer)
            {
                playerMovementController.GetAnimator().animator.SetLookAtWeight(weight, bodyWeight, headWeight);
                lookAtRay = new Ray(transform.position, playerMovementController._mainCamera.transform.forward);
                playerMovementController.GetAnimator().animator.SetLookAtPosition(lookAtRay.GetPoint(distance));
            }
        }
    }
}
