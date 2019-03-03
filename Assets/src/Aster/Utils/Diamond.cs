using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aster.Utils {

public class Diamond
{
    public Vector3Int center;
    public int r;
    public Diamond(Vector3Int center, int radius)
    {
        this.center = center;
        r = radius;
    }

    public bool Within(Vector3Int point)
    {
        return Metrics.DiamondDistance(center, point) <= r;
    }

    public int GetVolume()
    {
        int diamondCircumference = r * 4;
        int upperHalfSurface = 1 + 2 * r * (1 + r);
        return diamondCircumference + 2 * upperHalfSurface;
    }
}

}
