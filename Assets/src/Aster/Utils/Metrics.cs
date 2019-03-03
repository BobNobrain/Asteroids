using System;
using UnityEngine;

namespace Aster.Utils {

public static class Metrics
{
    public static int DiamondDistance(Vector3Int a, Vector3Int b)
    {
        int dx = a.x - b.x;
        int dy = a.y - b.y;
        int dz = a.z - b.z;
        return Math.Abs(dx) + Math.Abs(dy) + Math.Abs(dz);
    }
}

}
