using System.Collections.Generic;
using UnityEngine;

namespace Aster.Objects.Rope {

[RequireComponent(typeof(SpringJoint))]
[RequireComponent(typeof(LineRenderer))]
public class RopeGun: MonoBehaviour
{
    [Range(.1f, 5f)]
    public float ShootForce = 1f;

    public GameObject RopeSegmentPrefab;
    public GameObject RopeProjectilePrefab;
    public Transform BulletSpawn;

    public Transform RopeObject;

    [Range(2, 50)]
    public int MaxRopeSegments;

    private int segmentsSpawned = 0;
    private List<Transform> RopeTransforms;
    private Rigidbody lastSpawned = null;

    private SpringJoint selfJoint;
    private LineRenderer ropeRenderer;

    public void Awake()
    {
        selfJoint = GetComponent<SpringJoint>();
        ropeRenderer = GetComponent<LineRenderer>();
        RopeTransforms = new List<Transform>(MaxRopeSegments);
    }

    public void Update()
    {
        // update line renderer points
        if (ropeRenderer.positionCount != RopeTransforms.Count + 1)
        {
            ropeRenderer.positionCount = RopeTransforms.Count + 1;
        }
        for (int i = 0; i < RopeTransforms.Count; i++)
        {
            ropeRenderer.SetPosition(i, RopeTransforms[i].position);
        }
        ropeRenderer.SetPosition(RopeTransforms.Count, transform.position);

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
            BulletSpawn.position,
            Quaternion.LookRotation(BulletSpawn.forward, BulletSpawn.up),
            RopeObject
        );
        lastSpawned = projectile.GetComponent<Rigidbody>();

        // shoot projectile away
        lastSpawned.AddForce(transform.forward * ShootForce, ForceMode.Impulse);

        RopeTransforms.Add(projectile.transform);
    }

    public void SpawnRopeSegment()
    {
        if (segmentsSpawned >= MaxRopeSegments)
        {
            Debug.Log("Can't spawn more segments, limit exceeded");

            PullGunItself();

            return;
        }

        var ropeSegment = Instantiate(RopeSegmentPrefab, transform.position, Quaternion.identity, RopeObject);

        var joint = ropeSegment.GetComponent<SpringJoint>();
        segmentsSpawned += 1;
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
