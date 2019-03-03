using UnityEngine;

namespace Aster.World.Generation {

public class CubeMeshGenerator
{
    private static readonly Vector3[] openFaceDirections = new Vector3[] {
        new Vector3(+1, 0, 0),
        new Vector3(0, 0, +1),
        new Vector3(-1, 0, 0),
        new Vector3(0, 0, -1)
    };

    private int r;
    private int fullFaceVertices;
    private int fullFaceTriangles;
    private int openFaceVertices;
    private int openFaceTriangles;
    private int glueTriangles;

    private Vector3[] vs = null;
    private Vector2[] uvs = null;
    private int[] ts = null;

    private int accVs = 0, accTs = 0;

    public CubeMeshGenerator() {}

    public void GenerateCube(Mesh target, int r)
    {
        this.r = r;
        fullFaceVertices = r * r;
        fullFaceTriangles = 3 * 2 * (r - 1) * (r - 1);
        openFaceVertices = (r - 1) * (r - 2);
        openFaceTriangles = 3 * 2 * (r - 2) * (r - 3);
        glueTriangles = 3 * (3 * r - 5);

        vs = new Vector3[2 * fullFaceVertices + 4 * openFaceVertices];
        uvs = new Vector2[vs.Length];
        ts = new int[2 * fullFaceTriangles + 4 * (openFaceTriangles + glueTriangles)];

        accVs = 0;
        accTs = 0;

        GenerateFullFace(.5f);
        GenerateFullFace(-.5f);

        for (int i = 0; i < 4; i++)
        {
            GenerateOpenFace(openFaceDirections[i]);
            // accVs += openFaceVertices;
            // accTs += openFaceTriangles;
        }

        // for (int i = 0; i < 4; i++)
        // {
        //     GlueOpenFace(ts, accTs, uvs, accVs, openFaceDirections[i]);
        //     accTs += glueTriangles;
        // }

        target.Clear();
        target.vertices = vs;
        target.triangles = ts;
        target.uv = uvs;
        target.RecalculateNormals();
        // target.RecalculateBounds();
        target.RecalculateTangents();
    }

    private void GenerateFullFace(float y)
    {
        int i = accVs;
        int trI = accTs;

        float rMinus1 = r - 1f;

        for (int x = 0; x < r; x++)
        {
            for (int z = 0; z < r; z++)
            {
                // next point on unit cube
                float normx = x / rMinus1, normz = z / rMinus1;

                float px = normx - .5f;
                float pz = normz - .5f;
                if (y < 0)
                {
                    px = -px;
                }
                Vector3 point = new Vector3(px, y, pz);
                vs[i] = point;
                uvs[i] = new Vector2(normx, normz);

                // add triangles for Rect((x, y):(x+1, y+1)), except the last rows of x and y
                if (x != r - 1 && z != r - 1)
                {
                    ts[trI] = i;
                    ts[trI + 1] = i + r + 1;
                    ts[trI + 2] = i + r;

                    ts[trI + 3] = i;
                    ts[trI + 4] = i + 1;
                    ts[trI + 5] = i + r + 1;
                    trI += 6;
                }
                ++i;
            }
        }

        accVs += fullFaceVertices;
        accTs += fullFaceTriangles;
    }

    private void GenerateOpenFace(
        Vector3 zAxis
    )
    {
        int i = accVs;
        int trI = accTs;

        float rMinus1 = r - 1f;

        Vector3 yAxis = new Vector3(0, 1, 0);
        Vector3 xAxis = Vector3.Cross(zAxis, yAxis);

        int h = r - 2;

        for (int x = 0; x < r - 1; x++)
        {
            for (int y = 1; y < r - 1; y++)
            {
                // Debug.Log("Loop: " + x.ToString() + ", " + y.ToString() + "; trI = " + trI);
                // next point on unit cube
                float normx = x / rMinus1, normy = y / rMinus1;

                float px = normx - .5f;
                float py = normy - .5f;
                Vector3 point = px * xAxis + py * yAxis + .5f * zAxis;
                // Debug.Log(point);
                vs[i] = point;
                uvs[i] = new Vector2(normx, normy);

                // add triangles for Rect((x, y):(x+1, y+1)), except the last rows of x and y
                if (x < r - 2 && y < r - 2)
                {
                    ts[trI] = i;
                    ts[trI + 1] = i + 1;
                    ts[trI + 2] = i + h;

                    ts[trI + 3] = i + 1;
                    ts[trI + 4] = i + h + 1;
                    ts[trI + 5] = i + h;
                    trI += 6;
                }
                ++i;
            }
        }

        accVs += openFaceVertices;
        accTs += openFaceTriangles;
    }

    private static void GlueOpenFace(
        int[] ts, int tsOffset,
        Vector2[] uvs, int uvsOffset,
        Vector3 zAxis
    )
    {
        // top
    }
}

}
