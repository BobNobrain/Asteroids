using UnityEngine;

namespace Aster {
namespace Player {

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement: MonoBehaviour
{
    [Range(1e-2f, 1f)]
    public float CamSpeed = .1f;

    [Range(.1f, 10f)]
    public float VCamSpeedScale = 1f;

    public float Eps = 1e-3f;

    public float MaxMovementSpeed = 5f;
    public float MovementAccel = .5f;
    public float StrafeAccel = .4f;

    private float dMouseX, dMouseY;
    private float dMoveForward, dMoveRightward;

    private Rigidbody body;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        dMouseX = Input.GetAxisRaw("Mouse X");
        dMouseY = Input.GetAxisRaw("Mouse Y");

        dMoveForward = Input.GetAxisRaw("Vertical");
        dMoveRightward = Input.GetAxisRaw("Horizontal");
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

        Debug.Log(newForward);

        if (Mathf.Abs(dMouseY) > Eps)
            newForward = Vector3.RotateTowards(newForward, upward, VCamSpeedScale * CamSpeed * dMouseY, 0);
        if (Mathf.Abs(dMouseX) > Eps)
            newForward = Vector3.RotateTowards(newForward, rightward, CamSpeed * dMouseX, 0);

        transform.rotation = Quaternion.LookRotation(newForward, upward);
    }
    private void UpdateMovement()
    {
        var v = body.velocity;
        var forward = transform.forward;
        var rightward = transform.right;

        var f = forward * dMoveForward * MovementAccel + rightward * dMoveRightward * StrafeAccel;
        var a = f / body.mass;

        v += a;
        if (v.magnitude > MaxMovementSpeed)
        {
            v = v.normalized * MaxMovementSpeed;
        }

        body.velocity = v;
    }
}

}}
