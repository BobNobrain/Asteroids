using System.Collections.Generic;
using UnityEngine;

namespace Aster.Objects.Rope {

[RequireComponent(typeof(SpringJoint))]
[RequireComponent(typeof(LineRenderer))]
public class RopeGun: MonoBehaviour
{
    [Range(1f, 50f)]
    public float ShootForce = 10f;

    public GameObject RopeSegmentPrefab;
    public GameObject RopeProjectilePrefab;
    public Vector3 ProjectileSpawnOffset;

    public Transform RopeObject;

    [Range(2, 50)]
    public int MaxRopeSegments;

    public List<SpringJoint> RopeJoints;
    public List<Transform> RopeTransforms;
    public Rigidbody lastSpawned = null;

    private SpringJoint selfJoint;
    private LineRenderer ropeRenderer;

    public void Awake()
    {
        selfJoint = GetComponent<SpringJoint>();
        ropeRenderer = GetComponent<LineRenderer>();
    }

    public void Update()
    {
        // update line renderer points
        if (ropeRenderer.positionCount != RopeTransforms.Count)
        {
            ropeRenderer.positionCount = RopeTransforms.Count;
        }
        for (int i = 0; i < RopeTransforms.Count; i++)
        {
            ropeRenderer.SetPosition(i, RopeTransforms[i].position);
        }

        if (lastSpawned != null) return;

        if (Input.GetButton("Fire1"))
        {
            SpawnRopeProjectile();
            SpawnRopeSegment();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        var body = other.GetComponent<Rigidbody>();
        if (body == lastSpawned) return;

        SpawnRopeSegment();
    }


    public void SpawnRopeProjectile()
    {
        var projectile = Instantiate(
            RopeProjectilePrefab,
            transform.position + ProjectileSpawnOffset,
            Quaternion.identity,
            RopeObject
        );
        lastSpawned = projectile.GetComponent<Rigidbody>();

        // shoot projectile away
        lastSpawned.AddForce(transform.forward * ShootForce, ForceMode.Impulse);

        RopeTransforms.Add(projectile.transform);
    }

    public void SpawnRopeSegment()
    {
        if (RopeJoints.Count >= MaxRopeSegments)
        {
            Debug.Log("Can't spawn more segments, limit exceeded");

            PullGunItself();

            return;
        }

        var ropeSegment = Instantiate(RopeSegmentPrefab, transform.position, Quaternion.identity, RopeObject);

        var joint = ropeSegment.GetComponent<SpringJoint>();
        RopeJoints.Add(joint);
        RopeTransforms.Add(ropeSegment.transform);

        joint.connectedBody = lastSpawned;

        lastSpawned = ropeSegment.GetComponent<Rigidbody>();
        // shoot rope segment away too
        lastSpawned.AddForce(transform.forward * ShootForce, ForceMode.Impulse);
    }

    public void PullGunItself()
    {
        selfJoint.connectedBody = lastSpawned;
    }
}

}
