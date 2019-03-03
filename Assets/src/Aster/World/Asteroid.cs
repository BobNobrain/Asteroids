using UnityEngine;
using Noise;

namespace Aster {
namespace World {

public class Asteroid: MonoBehaviour, ILODController
{
    new private SphereCollider collider;
    private Rigidbody body;

    [Range(2,256)]
    public int resolution = 10;
    public int maxResolution = 50;
    public int minResolution = 5;
    public float lodTreshold = .2f;

    public float radius = 2f;
    public float density = 1f;

    public Material material;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters = null;
    Face[] faces = null;

    public bool LiveReload = false;

    public int seed = 0;
    public float rotationSpeed = 0.2f;

    public RidgidNoiseGenerator ridgedNoise;
    public SimplexNoiseGenerator simplexNoise;

    void Awake()
    {
        collider = GetComponent<SphereCollider>();
        body = GetComponent<Rigidbody>();

        // initial rotation
        body.AddTorque(0, 0, rotationSpeed, ForceMode.VelocityChange);
    }

    public RidgidNoiseGenerator.Settings ridgedSettings = new RidgidNoiseGenerator.Settings {
        numLayers = 4,
        strength = 0.05f,
        baseRoughness = 1.2f,
        roughness = 2.3f,
        persistence = 0.5f,

        centre = Vector3.zero,
        weightMultiplier = 0.8f,
        minValue = 0
    };

    public SimplexNoiseGenerator.Settings simplexSettings = new SimplexNoiseGenerator.Settings {
        numLayers = 4,
        strength = 0.05f,
        baseRoughness = 1.2f,
        roughness = 2.3f,
        persistence = 0.5f,
        centre = Vector3.zero,
        minValue = 0
    };

    public void Init()
    {
        ridgedNoise = new RidgidNoiseGenerator(ridgedSettings, seed);
        simplexNoise = new SimplexNoiseGenerator(simplexSettings, seed);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        if (faces == null || faces.Length == 0)
        {
            faces = new Face[6];
        }
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;
                meshObj.transform.position = transform.position;
                meshObj.transform.rotation = transform.rotation;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = material;
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            faces[i] = new Face(this, meshFilters[i].sharedMesh, directions[i]);
        }

        body.mass = radius * radius * density;
    }

    private void GenerateMesh()
    {
        float minh = 1e5f, maxh = 0;
        foreach (var face in faces)
        {
            var next = face.ConstructMesh(resolution);
            if (next.x < minh) minh = next.x;
            if (next.y > maxh) maxh = next.y;
        }

        collider.radius = minh + (maxh - minh) * .9f;
    }

    private float GetPointHeight(Vector3 unit)
    {
        // return (1 + ridgedNoise.Eval(unit)) * this.radius;
        return (1 + ridgedNoise.Eval(unit) + simplexNoise.Eval(unit)) * this.radius;
    }

    public void Generate()
    {
        // Init();
        GenerateMesh();
    }

    public void SetLOD(float percent)
    {
        if (percent < lodTreshold)
        {
            resolution = minResolution;
        }
        else
        {
            percent = (percent - lodTreshold) / (1 - lodTreshold);
            resolution = minResolution + (int) ((maxResolution - minResolution) * percent);
        }
        Generate();
    }

    private class Face
    {
        Mesh mesh;
        Vector3 localUp;
        Vector3 axisA;
        Vector3 axisB;
        Asteroid parent;

        public Face(Asteroid parent, Mesh mesh, Vector3 localUp)
        {
            this.parent = parent;
            this.mesh = mesh;
            this.localUp = localUp;

            axisA = new Vector3(localUp.y, localUp.z, localUp.x);
            axisB = Vector3.Cross(localUp, axisA);
        }

        public Vector2 ConstructMesh(int resolution)
        {
            Vector3[] vertices = new Vector3[resolution * resolution];
            Vector2[] uvs = new Vector2[resolution * resolution];
            int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
            int triIndex = 0;

            float minHeight = 1e5f;
            float maxHeight = 0;

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    int i = x + y * resolution;
                    Vector2 percent = new Vector2(x, y) / (resolution - 1);
                    Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                    Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                    float pointHeight = parent.GetPointHeight(pointOnUnitSphere);
                    if (pointHeight < minHeight) minHeight = pointHeight;
                    if (pointHeight > maxHeight) maxHeight = pointHeight;

                    vertices[i] = pointOnUnitSphere * pointHeight;
                    uvs[i] = new Vector2(x, y) / resolution;

                    if (x != resolution - 1 && y != resolution - 1)
                    {
                        triangles[triIndex] = i;
                        triangles[triIndex + 1] = i + resolution + 1;
                        triangles[triIndex + 2] = i + resolution;

                        triangles[triIndex + 3] = i;
                        triangles[triIndex + 4] = i + 1;
                        triangles[triIndex + 5] = i + resolution + 1;
                        triIndex += 6;
                    }
                }
            }
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            return new Vector2(minHeight, maxHeight);
        }
    }
}

}}
