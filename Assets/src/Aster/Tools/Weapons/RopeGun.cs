using System.Collections.Generic;
using UnityEngine;
using Aster.Objects;

namespace Aster.Tools.Weapons {

[RequireComponent(typeof(FixedJoint))]
public class RopeGun: MonoBehaviour, IWeapon
{
    #region Parameters
    // [Range(.1f, 5f)]
    public float ShootForce = 1f;

    public GameObject RopeSegmentPrefab;
    public GameObject RopeAnchorPrefab;
    public GameObject RopePrefab;

    public Transform BulletSpawn;


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

    private FixedJoint selfJoint;
    private Transform bulletsRoot;

    private Rigidbody playerBody;
    private FixedJoint playerJoint;

    private Rope activeRope;
    #endregion

    void Awake()
    {
        selfJoint = null;
        playerJoint = GetComponent<FixedJoint>();
    }

    void OnTriggerExit(Collider other)
    {
        other.enabled = false; // disable rope segment end trigger
        SpawnRopeSegment();
    }


    #region IWeapon implementation
    public void Init(GameObject player, GameObject bulletsRoot)
    {
        this.bulletsRoot = bulletsRoot.transform;
        playerBody = player.GetComponent<Rigidbody>();
        playerJoint.connectedBody = playerBody;
    }

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
                // activeRope = null;
                // // TODO: dispose the rope somehow?
                // state = State.FREE;
                break;

            case State.CONNECTED:
                // TODO: release the rope
                DetachGunFromRope();
                activeRope = null;
                state = State.FREE;
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
            BulletSpawn.position + BulletSpawn.forward * 2,
            BulletSpawn.rotation,
            bulletsRoot
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
            bulletsRoot
        );

        activeRope = rope.GetComponent<Rope>();
        activeRope.Init(MaxRopeSegments);
        activeRope.AttachToHead(ropeAnchor);
    }

    /// <summary>
    /// Instantiates a new rope segment and attaches it to activeRope.
    /// If rope segment limit exceeded, calls EndRopeAndPullGun instead
    /// </summary>
    private void SpawnRopeSegment()
    {
        if (segmentsSpawned >= MaxRopeSegments)
        {
            AttachGunToRope();
            return;
        }

        var ropeSegment = Instantiate(
            RopeSegmentPrefab,
            BulletSpawn.position,
            BulletSpawn.rotation,
            bulletsRoot
        );
        var segment = activeRope.AttachSegment(ropeSegment);

        // shoot rope segment away too
        // segment.body.AddForce(transform.forward * ShootForce, ForceMode.Impulse);

        segmentsSpawned += 1;
    }

    /// <summary>
    /// Connects this gun to the activeRope end and sets this.state to connected
    /// </summary>
    private void AttachGunToRope()
    {
        selfJoint = gameObject.AddComponent<FixedJoint>();
        // selfJoint.spring = 1e20f;
        // selfJoint.damper = 0.2f;
        // selfJoint.breakForce = float.PositiveInfinity;

        activeRope.AttachCargo(BulletSpawn, GetComponent<Rigidbody>(), selfJoint);
        state = State.CONNECTED;
    }

    private void DetachGunFromRope()
    {
        Destroy(selfJoint);
        selfJoint = null;
    }
    #endregion
}

}
