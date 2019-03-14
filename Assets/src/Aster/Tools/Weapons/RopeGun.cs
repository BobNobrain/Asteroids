using System.Collections.Generic;
using UnityEngine;
using Aster.Objects;

namespace Aster.Tools.Weapons {

[RequireComponent(typeof(SpringJoint))]
public class RopeGun: MonoBehaviour, IWeapon
{
    #region Parameters
    [Range(.1f, 5f)]
    public float ShootForce = 1f;

    public GameObject RopeSegmentPrefab;
    public GameObject RopeAnchorPrefab;
    public GameObject RopePrefab;

    public Transform BulletSpawn;

    public Transform ObjectsRoot;

    [Range(2, 50)]
    public int MaxRopeSegments;
    #endregion

    #region State
    private enum State
    {
        FREE, // nothing happened
        EJECTING_ROPE, // rope is being ejected at the moment
        CONNECTED // rope was ejected
    };

    private State state = State.FREE;

    private int segmentsSpawned = 0;

    private SpringJoint selfJoint;

    private Rope activeRope;
    #endregion

    public void Awake()
    {
        selfJoint = GetComponent<SpringJoint>();
    }

    public void Update()
    {
        // TODO: move this from here to player controller
        if (Input.GetButton("Fire1"))
        {
            PrimaryTrigger();
        }
    }

    void OnTriggerExit(Collider other)
    {
        other.enabled = false; // disable rope segment end trigger
        SpawnRopeSegment();
    }


    #region IWeapon implementation
    public void PrimaryTrigger()
    {
        switch (state)
        {
            case State.FREE:
                var anchor = SpawnRopeAnchor();
                SpawnRope(anchor);
                SpawnRopeSegment();

                state = State.EJECTING_ROPE;
                break;

            case State.EJECTING_ROPE:
                // TODO: cut the rope being ejected
                break;

            case State.CONNECTED:
                // TODO: release the rope
                break;
        }
    }

    public void SecondaryTrigger()
    {
        // TODO: pull the rope
    }
    #endregion


    #region Low-Level management

    /// <summary>
    /// Instantiates rope anchor object at BulletSpawn and pulls it forward
    /// </summary>
    /// <returns>Instantiated anchor's RigidBody component</returns>
    private Rigidbody SpawnRopeAnchor()
    {
        var projectile = Instantiate(
            RopeAnchorPrefab,
            BulletSpawn.position,
            Quaternion.LookRotation(BulletSpawn.forward, BulletSpawn.up),
            ObjectsRoot
        );
        var body = projectile.GetComponent<Rigidbody>();

        // shoot projectile away
        body.AddForce(transform.forward * ShootForce, ForceMode.Impulse);
        return body;
    }

    /// <summary>
    /// Instantiates a new Rope at BulletSpawn and attaches it to anchor provided.
    /// The rope spawned is stored into activeRope field.
    /// </summary>
    /// <param name="ropeAnchor">Rope anchor to attach the rope to</param>
    private void SpawnRope(Rigidbody ropeAnchor)
    {
        var rope = Instantiate(
            RopePrefab,
            BulletSpawn.position,
            BulletSpawn.rotation,
            ObjectsRoot
        );

        activeRope = rope.GetComponent<Rope>();
        activeRope.Init(MaxRopeSegments);
        activeRope.AttachTo(ropeAnchor);
    }

    /// <summary>
    /// Instantiates a new rope segment and attaches it to activeRope.
    /// If rope segment limit exceeded, calls EndRopeAndPullGun instead
    /// </summary>
    private void SpawnRopeSegment()
    {
        if (segmentsSpawned >= MaxRopeSegments)
        {
            EndRopeAndPullGun();
            return;
        }

        var ropeSegment = Instantiate(RopeSegmentPrefab, transform.position, Quaternion.identity, ObjectsRoot);
        var body = activeRope.AttachSegment(ropeSegment);

        // shoot rope segment away too
        body.AddForce(transform.forward * ShootForce, ForceMode.Impulse);

        segmentsSpawned += 1;
    }

    /// <summary>
    /// Connects this gun to the activeRope end and sets this.state to connected
    /// </summary>
    private void EndRopeAndPullGun()
    {
        activeRope.AttachSpringJointToRope(selfJoint);
        state = State.CONNECTED;
    }
    #endregion
}

}
