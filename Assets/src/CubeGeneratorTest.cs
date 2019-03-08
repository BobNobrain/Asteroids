using UnityEngine;
using UnityToolbag;
using Aster.World.Generation;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CubeGeneratorTest: MonoBehaviour, CubeMeshGenerator.IHeightProvider
{
    [Range(2, 50)]
    public int resolution = 3;

    public Material material;

    private MeshFilter filter;
    private Mesh mesh;
    private CubeMeshGenerator g;

    public float GetHeight(Vector3 onUnitSphere)
    {
        return 2f;
    }

    public void Awake()
    {
        GetComponent<MeshRenderer>().sharedMaterial = material;
        filter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        filter.sharedMesh = mesh;

        g = new CubeMeshGenerator(this);
        Generate();
    }

    public void Generate()
    {
        var promise = new Future<CubeMeshGenerator.CubeData>();

        promise.OnSuccess(cubeDataFuture => {
            var cubeData = cubeDataFuture.value;

            mesh.Clear();
            mesh.vertices = cubeData.vertices;
            mesh.triangles = cubeData.triangles;
            mesh.uv = cubeData.uvs;

            mesh.RecalculateNormals();
            // mesh.RecalculateBounds();
            mesh.RecalculateTangents();
        });

        promise.Process(() => g.GenerateCube(resolution));
    }

    public void OnDrawGizmos()
    {
        if (mesh == null) return;
        foreach (var v in mesh.vertices)
        {
            Gizmos.DrawWireSphere(v, .01f);
        }
    }
}
