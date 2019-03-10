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

    /// <summary>
    /// Calculates if given point is inside IntCube with center in (0, 0, 0)
    /// </summary>
    /// <param name="r">IntCube radius</param>
    /// <param name="point">Given point</param>
    /// <returns>True if point is within the cube, false otherwise</returns>
    private static bool withinZeroCube(int r, Vector3Int point)
    {
        return Math.Abs(point.x) <= r && Math.Abs(point.y) <= r && Math.Abs(point.z) <= r;
    }

    /// <summary>
    /// Calculates difference between this IntCube and wiping IntCube
    /// </summary>
    /// <param name="wiping">The IntCube to subtract</param>
    /// <returns>Set of integer points that remain of this cube after subtracting wiping cube</returns>
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

    /// <summary>
    /// Calculates if point is inside IntCube with given radius and center
    /// </summary>
    /// <param name="cubeCenter">Cube center</param>
    /// <param name="cubeR">Cube radius</param>
    /// <param name="point">The point to test</param>
    /// <returns>True if point is inside IntCube, false otherwise</returns>
    public static bool WithinCube(Vector3Int cubeCenter, int cubeR, Vector3Int point)
    {
        return withinZeroCube(cubeR, point - cubeCenter);
    }

    /// <summary>
    /// Calculates if inner IntCube is fully inside outer IntCube
    /// </summary>
    /// <param name="outerCubeCenter">Center of outer IntCube</param>
    /// <param name="outerCubeR">Radius of outer IntCube</param>
    /// <param name="innerCubeCenter">Center of inner IntCube</param>
    /// <param name="innerCubeR">Radius of inner IntCube</param>
    /// <returns>True if inner cube is fully inside outer, false otherwise</returns>
    public static bool WithinCube(
        Vector3Int outerCubeCenter,
        int outerCubeR,
        Vector3Int innerCubeCenter,
        int innerCubeR
    )
    {
        int dx = Math.Abs(outerCubeCenter.x - innerCubeCenter.x);
        int dy = Math.Abs(outerCubeCenter.y - innerCubeCenter.y);
        int dz = Math.Abs(outerCubeCenter.z - innerCubeCenter.z);

        int max = dx;
        if (dy > max) max = dy;
        if (dz > max) max = dz;

        return max + innerCubeR <= outerCubeR;
    }
}

}
