using UnityEngine;

namespace Aster.World.Generation {

public class CubeMeshGenerator
{
    private static readonly Vector2[] openFaceDirections = new Vector2[] {
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(-1, 0),
        new Vector2(0, -1)
    };

    public static void GenerateCube(Mesh target, int r)
    {
        int fullFaceVertices = r * r;
        int fullFaceTriangles = 3 * 2 * (r - 1) * (r - 1);
        int openFaceVertices = (r - 1) * (r - 2);
        int openFaceTriangles = 3 * 2 * (r - 2) * (r - 3);
        int glueTriangles = 3 * (3 * r - 5);

        Vector3[] vs = new Vector3[2 * fullFaceVertices + 4 * openFaceVertices];
        Vector2[] uvs = new Vector2[vs.Length];
        int[] ts = new int[2 * fullFaceTriangles + 4 * (openFaceTriangles + glueTriangles)];

        int accVs = 0, accTs = 0;

        GenerateFullFace(r, vs, accVs, ts, accTs, uvs, accVs, .5f);
        accVs += fullFaceVertices;
        accTs += fullFaceTriangles;
        GenerateFullFace(r, vs, accVs, ts, accTs, uvs, accVs, -.5f);
        accVs += fullFaceVertices;
        accTs += fullFaceTriangles;

        for (int i = 0; i < 4; i++)
        {
            GenerateOpenFace(vs, accVs, ts, accTs, uvs, accVs, openFaceDirections[i]);
            accVs += openFaceVertices;
            accTs += openFaceTriangles;
        }

        for (int i = 0; i < 4; i++)
        {
            GlueOpenFace(ts, accTs, uvs, accVs, openFaceDirections[i]);
            accTs += glueTriangles;
        }

        target.Clear();
        target.vertices = vs;
        target.triangles = ts;
        target.uv = uvs;
        target.RecalculateNormals();
        // target.RecalculateBounds();
        target.RecalculateTangents();
    }

    private static void GenerateFullFace(
        int r,
        Vector3[] vs, int vsOffset,
        int[] ts, int tsOffset,
        Vector2[] uvs, int uvsOffset,
        float z
    )
    {
        int i = vsOffset;
        int trI = tsOffset;

        float rMinus1 = r - 1f;

        for (int x = 0; x < r; x++)
        {
            for (int y = 0; y < r; y++)
            {
                Debug.Log("Loop: " + x.ToString() + ", " + y.ToString() + "; trI = " + trI);
                // next point on unit cube
                float normx = x / rMinus1, normy = y / rMinus1;

                float px = normx - .5f;
                float py = normy - .5f;
                if (z < 0)
                {
                    px = -px;
                }
                Vector3 point = new Vector3(px, py, z).normalized;
                vs[i] = point;
                uvs[i] = new Vector2(normx, normy);

                // add triangles for Rect((x, y):(x+1, y+1)), except the last rows of x and y
                if (x != r - 1 && y != r - 1)
                {
                    ts[trI] = i;
                    ts[trI + 1] = i + r;
                    ts[trI + 2] = i + r + 1;

                    ts[trI + 3] = i;
                    ts[trI + 4] = i + r + 1;
                    ts[trI + 5] = i + 1;
                    trI += 6;
                }
                ++i;
            }
        }
    }

    private static void GenerateOpenFace(
        Vector3[] vs, int vsOffset,
        int[] ts, int tsOffset,
        Vector2[] uvs, int uvsOffset,
        Vector2 forward
    )
    {}

    private static void GlueOpenFace(
        int[] ts, int tsOffset,
        Vector2[] uvs, int uvsOffset,
        Vector2 forward
    )
    {}
}

}
