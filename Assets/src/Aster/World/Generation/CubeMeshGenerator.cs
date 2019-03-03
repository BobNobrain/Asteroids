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

    public interface IHeightProvider
    {
        float GetHeight(Vector3 onUnitSphere);
    }

    private IHeightProvider provider;

    private int r;
    private int fullFaceVertices;
    private int fullFaceTriangles;
    private int openFaceVertices;
    private int openFaceTriangles;
    private int glueTriangles;

    private Vector3[] vs = null;
    private Vector2[] uvs = null;
    private int[] ts = null;
    private int[] edges = null;

    private int accVs = 0, accTs = 0, accEdges = 0;

    public CubeMeshGenerator(IHeightProvider prov)
    {
        provider = prov;
    }

    public void GenerateCube(Mesh target, int r)
    {
        this.r = r;
        fullFaceVertices = r * r;
        fullFaceTriangles = 6 * (r - 1) * (r - 1);

        openFaceVertices = (r - 1) * (r - 2);
        openFaceTriangles = 6 * (r - 2) * (r - 3);

        glueTriangles = 6 * (3 * r - 5);

        vs = new Vector3[2 * fullFaceVertices + 4 * openFaceVertices];
        uvs = new Vector2[vs.Length];
        ts = new int[2 * fullFaceTriangles + 4 * (openFaceTriangles + glueTriangles)];

        edges = new int[r * 8];

        Debug.Log("Total vertices: " + vs.Length);
        Debug.Log("Total triangles: " + ts.Length);

        accVs = 0;
        accTs = 0;

        GenerateFullFace(.5f);
        GenerateFullFace(-.5f);

        FillEdges();

        for (int i = 0; i < 4; i++)
        {
            GenerateOpenFace(i, openFaceDirections[i]);
        }

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
                Vector3 point = new Vector3(px, y, pz).normalized;
                vs[i] = point * provider.GetHeight(point);
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

    private void FillEdges()
    {
        int e = 0;
        // top full face
        for (int i = fullFaceVertices - r; i <= fullFaceVertices - 1; i++) { edges[e] = i; e++; }
        for (int i = r; i > 0; i--) { edges[e] = i * r - 1; e++; }
        for (int i = r - 1; i >= 0; i--) { edges[e] = i; e++; }
        for (int i = 0; i < r; i++) { edges[e] = i * r; e++; }
        // bottom full face
        for (int i = 0; i < r; i++) { edges[e] = i + fullFaceVertices; e++; }
        for (int i = 1; i <= r; i++) { edges[e] = i * r - 1 + fullFaceVertices; e++; }
        for (int i = fullFaceVertices - 1; i >= fullFaceVertices - r; i--) { edges[e] = i + fullFaceVertices; e++; }
        for (int i = r - 1; i >= 0; i--) { edges[e] = i * r + fullFaceVertices; e++; }
    }

    private void GenerateOpenFace(int faceIndex, Vector3 zAxis)
    {
        int i = accVs;
        int trI = accTs;

        float rMinus1 = r - 1f;

        Vector3 yAxis = new Vector3(0, 1, 0);
        Vector3 xAxis = Vector3.Cross(zAxis, yAxis);

        int h = r - 2;
        int nextOpenFaceIndex = (
            (
                accVs + openFaceVertices - 2 * fullFaceVertices
            ) % (4 * openFaceVertices)
        ) + 2 * fullFaceVertices;

        int topGlueEdgeOffset = faceIndex * r;
        int bottomGlueEdgeOffset = topGlueEdgeOffset + 4 * r;

        for (int x = 0; x < r - 1; x++)
        {
            // horizontal bottom glue
            if (x == r - 2)
            {
                // overlap to next open face

                ts[trI] = edges[bottomGlueEdgeOffset + x];
                ts[trI + 1] = i;
                ts[trI + 2] = edges[bottomGlueEdgeOffset + x + 1];
                ts[trI + 3] = edges[bottomGlueEdgeOffset + x + 1];
                ts[trI + 4] = i;
                ts[trI + 5] = nextOpenFaceIndex;
                trI += 6;
            }
            else
            {
                ts[trI] = edges[bottomGlueEdgeOffset + x];
                ts[trI + 1] = i;
                ts[trI + 2] = i + h;
                ts[trI + 3] = edges[bottomGlueEdgeOffset + x];
                ts[trI + 4] = i + h;
                ts[trI + 5] = edges[bottomGlueEdgeOffset + x + 1];
                trI += 6;
            }

            for (int y = 1; y < r - 1; y++)
            {
                // next point on unit cube
                float normx = x / rMinus1, normy = y / rMinus1;

                float px = normx - .5f;
                float py = normy - .5f;
                Vector3 point = (px * xAxis + py * yAxis + .5f * zAxis).normalized;
                vs[i] = point * provider.GetHeight(point);
                uvs[i] = new Vector2(normx, normy);

                // add triangles for Rect((x, y):(x+1, y+1)), except the last rows of x and y
                if (y < r - 2)
                {
                    if (x < r - 2)
                    {
                        ts[trI] = i;
                        ts[trI + 1] = i + 1;
                        ts[trI + 2] = i + h;

                        ts[trI + 3] = i + 1;
                        ts[trI + 4] = i + h + 1;
                        ts[trI + 5] = i + h;
                        trI += 6;
                    }
                    else if (x == r - 2)
                    {
                        // generate vertical glue
                        ts[trI] = nextOpenFaceIndex + y - 1;
                        ts[trI + 1] = i;
                        ts[trI + 2] = i + 1;

                        ts[trI + 3] = nextOpenFaceIndex + y;
                        ts[trI + 4] = nextOpenFaceIndex + y - 1;
                        ts[trI + 5] = i + 1;
                        trI += 6;
                    }
                }
                ++i;
            }

            // horizontal glue
            if (x == r - 2)
            {
                // overlap to next open face

                // top glue
                ts[trI] = edges[topGlueEdgeOffset + x];
                ts[trI + 1] = edges[topGlueEdgeOffset + x + 1];
                ts[trI + 2] = i - 1;
                ts[trI + 3] = edges[topGlueEdgeOffset + x + 1];
                ts[trI + 4] = nextOpenFaceIndex + h - 1;
                ts[trI + 5] = i - 1;
                trI += 6;

                // bottom glue
            }
            else
            {
                // top glue
                ts[trI] = edges[topGlueEdgeOffset + x];
                ts[trI + 1] = i - 1 + h;
                ts[trI + 2] = i - 1;
                ts[trI + 3] = edges[topGlueEdgeOffset + x];
                ts[trI + 4] = edges[topGlueEdgeOffset + x + 1];
                ts[trI + 5] = i - 1 + h;
                trI += 6;
            }
        }

        accVs += openFaceVertices;
        accTs += openFaceTriangles + glueTriangles;
    }
}

}
