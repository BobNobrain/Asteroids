using System.Collections.Generic;
using UnityEngine;

namespace Aster.Objects {

[RequireComponent(typeof(LineRenderer))]
public class Rope: MonoBehaviour
{
    public struct Segment
    {
        public Transform transform;
        public SpringJoint spring;
        public Rigidbody body;

        public Segment(GameObject from)
        {
            transform = from.transform;
            spring = from.GetComponent<SpringJoint>();
            body = from.GetComponent<Rigidbody>();
        }
    }

    private LineRenderer ropeRenderer;

    private List<Segment> ropeSegments;
    private Rigidbody head;
    private Segment cargo;

    void Awake()
    {
        ropeRenderer = GetComponent<LineRenderer>();
    }

    public void Init(int maxSize)
    {
        ropeSegments = new List<Segment>(maxSize);
    }

    public void Update()
    {
        // update line renderer points
        // TODO: more precise drawing to suit real colliders positions
        int desiredPointsCount = ropeSegments.Count;
        if (head != null) desiredPointsCount += 1;
        if (cargo.transform != null) desiredPointsCount += 1;

        if (ropeRenderer.positionCount != desiredPointsCount)
        {
            ropeRenderer.positionCount = desiredPointsCount;
        }
        int d = 0;
        if (head != null)
        {
            ropeRenderer.SetPosition(0, head.transform.position);
            d = 1;
        }
        for (int i = 0; i < ropeSegments.Count; i++)
        {
            ropeRenderer.SetPosition(i + d, ropeSegments[i].transform.position);
        }

        if (cargo.transform != null)
        {
            ropeRenderer.SetPosition(desiredPointsCount - 1, cargo.transform.position);
        }
    }

    /// <summary>
    /// Attaches a new rope segment to the end of this rope.
    /// Rope segment must have RigidBody and SpringJoint components
    /// </summary>
    /// <param name="ropeSegment">GameObject representing a segment to attach</param>
    /// <returns>Resulting segment structure</returns>
    public Segment AttachSegment(GameObject ropeSegment)
    {
        var segment = new Segment(ropeSegment);
        if (ropeSegments.Count > 0)
        {
            segment.spring.connectedBody = ropeSegments[ropeSegments.Count - 1].body;
        }
        else
        {
            segment.spring.connectedBody = head;
        }
        ropeSegments.Add(segment);
        return segment;
    }

    /// <summary>
    /// Sets given RigidBody as rope head
    /// </summary>
    /// <param name="body">RigidBody to attach this rope to</param>
    public void AttachToHead(Rigidbody body)
    {
        head = body;
        if (ropeSegments.Count > 0)
        {
            ropeSegments[0].spring.connectedBody = head;
        }
    }

    /// <summary>
    /// Attaches an external SpringJoint to last segment of the rope
    /// </summary>
    public void AttachCargo(Transform to, Rigidbody body, FixedJoint joint)
    {
        cargo = new Segment();
        cargo.transform = to;
        cargo.body = body;
        cargo.spring = null;

        joint.connectedBody = ropeSegments[ropeSegments.Count - 1].body;
    }

    public void Shrink()
    {
        // TODO
    }
}

}
