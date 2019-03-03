using UnityEngine;
using Aster.World.Generation;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CubeGeneratorTest : MonoBehaviour
{
    [Range(2, 50)]
    public int resolution = 3;

    public Material material;

    private MeshFilter filter;
    private Mesh mesh;

    public void Awake()
    {
        GetComponent<MeshRenderer>().sharedMaterial = material;
        filter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        filter.sharedMesh = mesh;
        Generate();
    }

    public void Generate()
    {
        CubeMeshGenerator.GenerateCube(mesh, resolution);
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
