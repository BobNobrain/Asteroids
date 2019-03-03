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

    private static bool withinZeroCube(int r, Vector3Int point)
    {
        return Math.Abs(point.x) <= r && Math.Abs(point.y) <= r && Math.Abs(point.z) <= r;
    }

    public List<Vector3Int> Diff(IntCube wiping)
    {
        var result = new List<Vector3Int>();

        // shifting coords so that this cube center is (0, 0, 0)
        var wcShifted  = wiping.center - center;

        // put these in stack to quicker access
        var wpR = wiping.radius;
        int cx = center.x, cy = center.y, cz = center.z;
        int wcx = wcShifted.x, wcy = wcShifted.y, wcz = wcShifted.z;

        // x, y and z are shifted coords
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    bool belongsToWiping = Math.Abs(x - wcx) <= wpR && Math.Abs(y - wcy) <= wpR && Math.Abs(z - wcz) <= wpR;
                    if (!belongsToWiping)
                    {
                        // point is not in wiping, so save it
                        result.Add(new Vector3Int(x + cx, y + cy, z + cz));
                    }
                }
            }
        }

        return result;
    }

    public static bool WithinCube(Vector3Int cubeCenter, int cubeR, Vector3Int point)
    {
        return withinZeroCube(cubeR, point - cubeCenter);
    }
}

}
