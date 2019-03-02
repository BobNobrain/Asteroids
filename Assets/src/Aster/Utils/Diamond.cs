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
        return IntegerDistance(center, point) <= r;
    }

    public int GetVolume()
    {
        int diamondCircumference = r * 4;
        int upperHalfSurface = 1 + 2 * r * (1 + r);
        return diamondCircumference + 2 * upperHalfSurface;
    }

    public static int IntegerDistance(Vector3Int a, Vector3Int b)
    {
        int dx = a.x - b.x;
        int dy = a.y - b.y;
        int dz = a.z - b.z;
        return Math.Abs(dx) + Math.Abs(dy) + Math.Abs(dz);
    }
}

}
