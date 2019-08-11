using UnityEngine;
using Aster.Tools.Weapons;

namespace Aster.Player {

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerMovement: MonoBehaviour
{
    [Range(1e-2f, 1f)]
    public float CamSpeed = .1f;

    [Range(.1f, 10f)]
    public float VCamSpeedScale = 1f;

    [Range(1e-3f, 1f)]
    public float CamCircleSpeed = .025f;

    public float Eps = 1e-3f;

    public float MaxMovementSpeed = 5f;
    public float MovementAccel = .5f;
    public float StrafeAccel = .4f;
    public float ShiftAccelScale = 2f;

    [Range(0, 5f)]
    public float AccelStaminaConsumption = .5f;

    private float dMouseX, dMouseY, dMouseZ;
    private float dMoveForward, dMoveRightward;
    private bool accelerate;

    private Rigidbody body;
    private PlayerStats stats;

    // TODO: refactor
    public RopeGun CurrentWeapon;
    public GameObject BulletsRoot;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        stats = GetComponent<PlayerStats>();
    }

    void Start()
    {
        if (CurrentWeapon != null)
        {
            // CurrentWeapon.Init(gameObject, BulletsRoot);
        }
    }

    void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            Debug.Log("Quit");
            Application.Quit();
            return;
        }

        // if (Input.GetButton("Fire1") && CurrentWeapon != null)
        // {
        //     CurrentWeapon.PrimaryTrigger();
        // }

        dMouseX = Input.GetAxisRaw("Mouse X");
        dMouseY = Input.GetAxisRaw("Mouse Y");

        dMouseZ = Input.GetAxis("Look Rotation");

        dMoveForward = Input.GetAxisRaw("Vertical");
        dMoveRightward = Input.GetAxisRaw("Horizontal");

        // acceleration is on when shift is pressed & we have stamina
        accelerate = Input.GetButton("Shift") && stats.Stamina.Acquire(Time.deltaTime * AccelStaminaConsumption);
    }

    void FixedUpdate()
    {
        UpdateCameraLook();
        UpdateMovement();
    }

    private void UpdateCameraLook()
    {
        var forward = transform.forward;
        var upward = transform.up;
        var rightward = transform.right;

        var newForward = forward;
        var newUpward = upward;

        if (Mathf.Abs(dMouseY) > Eps)
            newForward = Vector3.RotateTowards(newForward, upward, VCamSpeedScale * CamSpeed * dMouseY, 0);
        if (Mathf.Abs(dMouseX) > Eps)
            newForward = Vector3.RotateTowards(newForward, rightward, CamSpeed * dMouseX, 0);

        if (Mathf.Abs(dMouseZ) > Eps)
            newUpward = Vector3.RotateTowards(newUpward, rightward, CamCircleSpeed * dMouseZ, 0);

        transform.rotation = Quaternion.LookRotation(newForward, newUpward);
    }
    private void UpdateMovement()
    {
        var v = body.velocity;
        var forward = transform.forward;
        var rightward = transform.right;

        var f = forward * dMoveForward * MovementAccel + rightward * dMoveRightward * StrafeAccel;
        if (accelerate) f *= ShiftAccelScale;
        body.AddForce(f, ForceMode.Impulse);

        v = body.velocity;
        float max = MaxMovementSpeed;
        if (accelerate) max *= ShiftAccelScale;

        float real = v.magnitude;
        if (real > max)
        {
            v = v.normalized * Mathf.Lerp(real, max, max * .05f);
            body.velocity = v;
        }
    }
}

}
