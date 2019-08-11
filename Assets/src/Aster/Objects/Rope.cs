using System.Collections.Generic;
using UnityEngine;

namespace Aster.Objects {

[RequireComponent(typeof(LineRenderer))]
public class Rope: MonoBehaviour
{
    private LineRenderer ropeRenderer;

    private List<Transform> ropePointTransforms;
    private List<Rigidbody> ropePointBodies;

    public int PointCount
    { get { return ropePointTransforms == null ? 0 : ropePointTransforms.Count; } }

    public int MaxPoints;
    public GameObject PointPrefab;
    public GameObject Head;

    public FixedJoint CargoJoint;

    void Awake()
    {
        ropeRenderer = GetComponent<LineRenderer>();
        ropePointBodies = new List<Rigidbody>(MaxPoints);
        ropePointTransforms = new List<Transform>(MaxPoints);

        ropePointTransforms.Add(Head.transform);
        ropePointBodies.Add(Head.GetComponent<Rigidbody>());
    }

    public void Update()
    {
        // update line renderer points
        int desiredPointsCount = ropePointTransforms.Count;

        if (ropeRenderer.positionCount != desiredPointsCount)
        {
            ropeRenderer.positionCount = desiredPointsCount;
        }

        for (int i = 0; i < ropePointTransforms.Count; i++)
        {
            ropeRenderer.SetPosition(i, ropePointTransforms[i].position);
        }
    }

    // TODO: (Vector3 at, ...)
    public void SpawnSegment(float distance)
    {
        Transform lastT = ropePointTransforms[ropePointTransforms.Count - 1];
        Vector3 at = lastT.position - lastT.forward * distance;

        var point = Instantiate(
            PointPrefab,
            at,
            lastT.rotation,
            transform
        );

        var joint = point.GetComponent<Joint>();
        if (joint != null)
        {
            joint.connectedBody = ropePointBodies[ropePointBodies.Count - 1];
        }

        ropePointTransforms.Add(point.transform);
        ropePointBodies.Add(point.GetComponent<Rigidbody>());
    }

    public void PullHead(Vector3 force)
    {
        ropePointBodies[0].AddForce(force, ForceMode.Impulse);
    }

    public void AttachCargo()
    {
        CargoJoint.connectedBody = ropePointBodies[ropePointBodies.Count - 1];
    }
}

}
