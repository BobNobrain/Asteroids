using UnityEngine;
using Noise;
using UnityToolbag;
using Aster.World.Generation;

namespace Aster.World {

public class Asteroid: MonoBehaviour, ILODController, CubeMeshGenerator.IHeightProvider
{
    new private SphereCollider collider;
    private Rigidbody body;
    private MeshFilter filter;
    private Mesh mesh;
    private CubeMeshGenerator meshGenerator;

    [Range(3,100)]
    public int resolution = 10;
    public int maxResolution = 50;
    public int minResolution = 5;
    public float lodTreshold = .2f;

    public float radius = 2f;
    public float density = 1f;

    public Material material;

    public bool LiveReload = false;

    public int seed = 0;
    public float rotationSpeed = 0.2f;

    public RidgidNoiseGenerator ridgedNoise;
    public SimplexNoiseGenerator simplexNoise;

    void Awake()
    {
        collider = GetComponent<SphereCollider>();
        body = GetComponent<Rigidbody>();

        filter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        filter.sharedMesh = mesh;

        meshGenerator = new CubeMeshGenerator(this);

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

        GetComponent<MeshRenderer>().sharedMaterial = material;

        body.mass = radius * radius * density;
    }

    private void GenerateMeshInBackground()
    {
        var promise = new Future<CubeMeshGenerator.CubeData>();

        promise.OnSuccess(cubeDataFuture => {
            var cubeData = cubeDataFuture.value;

            float minh = cubeData.minmax.x, maxh = cubeData.minmax.y;
            collider.radius = minh + (maxh - minh) * .9f;

            mesh.Clear();
            mesh.vertices = cubeData.vertices;
            mesh.triangles = cubeData.triangles;
            mesh.uv = cubeData.uvs;

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
        });

        promise.Process(() => meshGenerator.GenerateCube(resolution));
    }

    public float GetHeight(Vector3 unit)
    {
        return (1 + ridgedNoise.Eval(unit) + simplexNoise.Eval(unit)) * this.radius;
    }

    public void Generate()
    {
        var cubeData = meshGenerator.GenerateCube(resolution);

        float minh = cubeData.minmax.x, maxh = cubeData.minmax.y;

        Dispatcher.InvokeAsync(() => {
            if (this == null) return;

            collider.radius = minh + (maxh - minh) * .9f;

            mesh.Clear();
            mesh.vertices = cubeData.vertices;
            mesh.triangles = cubeData.triangles;
            mesh.uv = cubeData.uvs;

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
        });
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
}

}
