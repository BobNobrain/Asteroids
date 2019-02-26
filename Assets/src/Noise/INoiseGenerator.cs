using UnityEngine;

namespace Noise {
public interface INoiseGenerator
{
    float Eval(Vector3 point);
}
}
