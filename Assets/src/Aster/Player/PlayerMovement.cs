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

    private float dMouseX, dMouseY;

    private Rigidbody body;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        dMouseX = Input.GetAxisRaw("Mouse X");
        dMouseY = Input.GetAxisRaw("Mouse Y");
    }

    void FixedUpdate()
    {
        UpdateCameraLook();
        UpdateMovement();
    }

    private void UpdateCameraLook() {
        var forward = transform.forward;
        var upward = transform.up;
        var rightward = transform.right;

        var newForward = forward;

        Debug.Log(newForward);

        if (Mathf.Abs(dMouseY) > Eps)
            newForward = Vector3.RotateTowards(newForward, upward, VCamSpeedScale * CamSpeed * dMouseY, 0);
        if (Mathf.Abs(dMouseX) > Eps)
            newForward = Vector3.RotateTowards(newForward, rightward, CamSpeed * dMouseX, 0);

        // var angles = Quaternion.LookRotation(forward, upward).eulerAngles;
        // angles.y += dMouseX * HCamSpeed;
        // angles.x -= dMouseY * VCamSpeed;

        // if (angles.x > 360) angles.x -= 360;
        // if (angles.x < 0) angles.x += 360;

        // if (angles.y > 360) angles.y -= 360;
        // if (angles.y < 0) angles.y += 360;

        transform.rotation = Quaternion.LookRotation(newForward, upward);
    }
    private void UpdateMovement() {}
}

}}
