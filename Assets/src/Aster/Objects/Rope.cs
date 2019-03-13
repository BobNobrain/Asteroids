using System.Collections.Generic;
using UnityEngine;

namespace Aster.Objects {

public class Rope: MonoBehaviour
{
    public class RopePoint
    {
        public Transform transform;
        public Rigidbody body;
        public SpringJoint spring;

        public RopePoint(GameObject obj)
        {
            transform = obj.transform;
            body = obj.GetComponent<Rigidbody>();
            spring = obj.GetComponent<SpringJoint>();
        }
    }

    [Range(0.5f, 5f)]
    public float SectionLength = 1f;

    public List<RopePoint> points;

    public GameObject RopePointPrefab;
}

}

