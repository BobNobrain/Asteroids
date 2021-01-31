using UnityEngine;
using Aster.Utils;

namespace Aster.Player
{

public class ZeroGMover: PlayerMover
{
    private Settings settings;
    private static readonly float Eps = 1e-3f;

    #region Inputs
    private float dMouseX, dMouseY, dMouseZ;
    private float dMoveForward, dMoveRightward;
    private bool accelerate;
    #endregion

    public ZeroGMover(Settings settings, PlayerController player, FocusableInput input): base(player, input)
    {
        this.settings = settings;
    }

    // public override void Update()
    // {
    //     dMouseX = input.GetAxis("Mouse X");
    //     dMouseY = input.GetAxis("Mouse Y");

    //     dMouseZ = input.GetAxis("Look Rotation");

    //     dMoveForward = input.GetAxisRaw("Vertical");
    //     dMoveRightward = input.GetAxisRaw("Horizontal");

    //     // acceleration is on when shift is pressed & we have stamina
    //     accelerate = input.GetButton("Shift") && player.stats.Stamina.Acquire(
    //         Time.deltaTime * settings.AccelStaminaConsumption
    //     );
    // }

    public override void Clear()
    {
        dMouseX = 0f;
        dMouseY = 0f;
        dMouseZ = 0f;

        dMoveForward = 0f;
        dMoveRightward = 0f;
        accelerate = false;
    }

    public override void FixedUpdate()
    {
        dMouseX = input.GetAxis("Mouse X");
        dMouseY = input.GetAxis("Mouse Y");

        dMouseZ = input.GetAxis("Look Rotation");

        dMoveForward = input.GetAxisRaw("Vertical");
        dMoveRightward = input.GetAxisRaw("Horizontal");

        // acceleration is on when shift is pressed & we have stamina
        accelerate = input.GetButton("Shift") && player.stats.Stamina.Acquire(
            Time.deltaTime * settings.AccelStaminaConsumption
        );

        UpdateCameraLook();
        UpdateMovement();
        // Clear();
    }

    private void UpdateCameraLook()
    {
        var forward = player.transform.forward;
        var upward = player.transform.up;
        var rightward = player.transform.right;

        var newForward = forward;
        var newUpward = upward;

        if (Mathf.Abs(dMouseY) > Eps)
        {
            newForward = Vector3.RotateTowards(
                newForward,
                upward,
                settings.VCamSpeedScale * settings.CamSpeed * dMouseY, 0);
        }
        if (Mathf.Abs(dMouseX) > Eps)
        {
            newForward = Vector3.RotateTowards(newForward, rightward, settings.CamSpeed * dMouseX, 0);
        }

        if (Mathf.Abs(dMouseZ) > Eps)
            newUpward = Vector3.RotateTowards(newUpward, rightward, settings.CamCircleSpeed * dMouseZ, 0);

        player.transform.rotation = Quaternion.LookRotation(newForward, newUpward);
    }
    private void UpdateMovement()
    {
        var v = player.body.velocity;
        var forward = player.transform.forward;
        var rightward = player.transform.right;

        var f = forward * dMoveForward * settings.MovementAccel + rightward * dMoveRightward * settings.StrafeAccel;
        if (accelerate) f *= settings.ShiftAccelScale;
        player.body.AddForce(f, ForceMode.Impulse);

        v = player.body.velocity;
        float max = settings.MaxMovementSpeed;
        if (accelerate) max *= settings.ShiftAccelScale;

        float real = v.magnitude;
        if (real > max)
        {
            v = v.normalized * Mathf.Lerp(real, max, max * .05f);
            player.body.velocity = v;
        }
    }

    [System.Serializable]
    public class Settings
    {
        #region Camera Props
        [Range(1e-2f, 1f)]
        public float CamSpeed = .1f;

        [Range(.1f, 10f)]
        public float VCamSpeedScale = 1f;

        [Range(1e-3f, 1f)]
        public float CamCircleSpeed = .025f;

        [Range(0.5f, 10f)]
        public float maxGatherDistance = 2f;
        #endregion

        #region Movement Props
        public float MaxMovementSpeed = 5f;
        public float MovementAccel = .5f;
        public float StrafeAccel = .4f;
        public float ShiftAccelScale = 2f;

        [Range(0, 5f)]
        public float AccelStaminaConsumption = .5f;
        #endregion
    }
}

}
