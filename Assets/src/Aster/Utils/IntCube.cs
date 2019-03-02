using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aster.Utils {

public class IntCube
{
    public Vector3Int center;
    public int radius;

    public IntCube(Vector3Int center, int radius)
    {
        this.center = center;
        this.radius = radius;
    }

    public int GetVolume()
    {
        int side = radius * 2 + 1;
        return side * side * side;
    }

    private static bool Within(int r, Vector3Int point)
    {
        return Math.Abs(point.x) <= r && Math.Abs(point.y) <= r && Math.Abs(point.z) <= r;
    }

    public List<Vector3Int> Diff(IntCube wiping)
    {
        var result = new List<Vector3Int>();
        var wcShifted  = wiping.center - center;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    var point = new Vector3Int(x, y, z);
                    if (!Within(wiping.radius, point - wcShifted))
                    {
                        result.Add(point);
                    }
                }
            }
        }

        return result;
    }
}

}
