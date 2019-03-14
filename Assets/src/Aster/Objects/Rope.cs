using System.Collections.Generic;
using UnityEngine;

namespace Aster.Objects {

[RequireComponent(typeof(LineRenderer))]
public class Rope: MonoBehaviour
{
    private LineRenderer ropeRenderer;

    private List<Transform> ropeTransforms;
    private Rigidbody lastSegmentBody;

    void Awake()
    {
        ropeRenderer = GetComponent<LineRenderer>();
    }

    public void Init(int maxSize)
    {
        ropeTransforms = new List<Transform>(maxSize);
        lastSegmentBody = null;
    }

    public void Update()
    {
        // update line renderer points
        // TODO: more precise drawing to suit real colliders positions
        if (ropeRenderer.positionCount != ropeTransforms.Count + 1)
        {
            ropeRenderer.positionCount = ropeTransforms.Count + 1;
        }
        for (int i = 0; i < ropeTransforms.Count; i++)
        {
            ropeRenderer.SetPosition(i, ropeTransforms[i].position);
        }
        ropeRenderer.SetPosition(ropeTransforms.Count, transform.position);
    }

    /// <summary>
    /// Attaches a new rope segment to the end of this rope.
    /// Rope segment must have RigidBody and SpringJoint components
    /// </summary>
    /// <param name="ropeSegment">GameObject representing a segment to attach</param>
    /// <returns>RigidBody of the segment (to optimize RopeGun calculations)</returns>
    public Rigidbody AttachSegment(GameObject ropeSegment)
    {
        ropeTransforms.Add(ropeSegment.transform);

        var joint = ropeSegment.GetComponent<SpringJoint>();
        joint.connectedBody = lastSegmentBody;

        lastSegmentBody = ropeSegment.GetComponent<Rigidbody>();
        return lastSegmentBody;
    }

    /// <summary>
    /// Attaches the beginning of this rope to a given RigidBody
    /// </summary>
    /// <param name="body">RigidBody to attach this rope to</param>
    public void AttachTo(Rigidbody body)
    {
        if (lastSegmentBody == null)
        {
            lastSegmentBody = body;
        }
        else
        {
            ropeTransforms[0].GetComponent<SpringJoint>().connectedBody = body;
        }
    }

    /// <summary>
    /// Attaches an external SpringJoint to last segment of the rope
    /// </summary>
    /// <param name="jointToAttach">SpringJoint that will be attached</param>
    public void AttachSpringJointToRope(SpringJoint jointToAttach)
    {
        jointToAttach.connectedBody = lastSegmentBody;
    }
}

}
