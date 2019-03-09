using UnityEngine;
using Noise;
using UnityToolbag;
using Aster.World.Generation;

namespace Aster.World {

public class Asteroid: MonoBehaviour, ILODController, CubeMeshGenerator.IHeightProvider
{
    private SphereCollider physicsCollider;
    private Rigidbody body;
    private MeshFilter filter;

    private Mesh[] meshes;
    private float[] colliderRadiuses;
    private bool[] meshesInitialized;

    private CubeMeshGenerator meshGenerator;
    private object _lock = new object();

    [HideInInspector] public Chunk region;

    private bool colliderEnabled;

    [Range(3,100)]
    public int resolution;
    public int maxResolution = 50;
    public int minResolution = 5;
    public int middleResolution = 20;
    public float lodLowTreshold = .2f;
    public float lodHighTreshold = .9f;

    public float radius = 2f;
    public float density = 1f;

    public Material material;

    public bool LiveReload = false;

    public int seed = 0;
    public float rotationSpeed = 0.2f;

    #region NoiseSettings
    public RidgidNoiseGenerator ridgedNoise;
    public SimplexNoiseGenerator simplexNoise;

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
    #endregion

    #region Initialization
    void Awake()
    {
        physicsCollider = GetComponent<SphereCollider>();
        body = GetComponent<Rigidbody>();

        meshes = new Mesh[]
        {
            new Mesh(),
            new Mesh(),
            new Mesh()
        };
        colliderRadiuses = new float[] { 0f, 0f, 0f };
        meshesInitialized = new bool[] { false, false, false };

        filter = GetComponent<MeshFilter>();
        filter.sharedMesh = meshes[0];

        meshGenerator = new CubeMeshGenerator(this);

        // initial rotation
        body.AddTorque(0, 0, rotationSpeed, ForceMode.VelocityChange);
    }

    public void Init()
    {
        ridgedNoise = new RidgidNoiseGenerator(ridgedSettings, seed);
        simplexNoise = new SimplexNoiseGenerator(simplexSettings, seed);

        GetComponent<MeshRenderer>().sharedMaterial = material;

        body.mass = radius * radius * density;
    }
    #endregion

    #region Generation
    public void Generate(int resolutionIndex, int resolution, bool forceRegenerate)
    {
        CubeMeshGenerator.CubeData cubeData = null;
        Mesh currentMesh = meshes[resolutionIndex];

        if (forceRegenerate || !meshesInitialized[resolutionIndex])
        {
            lock (_lock)
            {
                cubeData = meshGenerator.GenerateCube(resolution);
            }
        }

        Dispatcher.InvokeAsync(() => {
            if (this == null) return;

            if (colliderEnabled == physicsCollider.isTrigger) physicsCollider.isTrigger = !colliderEnabled;

            if (cubeData != null)
            {
                if (colliderEnabled)
                {
                    float minh = cubeData.minmax.x, maxh = cubeData.minmax.y;
                    colliderRadiuses[resolutionIndex] = minh + (maxh - minh) * .9f;
                }

                currentMesh.Clear();
                currentMesh.vertices = cubeData.vertices;
                currentMesh.triangles = cubeData.triangles;
                currentMesh.uv = cubeData.uvs;

                currentMesh.RecalculateNormals();
                currentMesh.RecalculateTangents();

                meshesInitialized[resolutionIndex] = true;
            }

            if (colliderEnabled)
            {
                physicsCollider.radius = colliderRadiuses[resolutionIndex];
            }

            filter.sharedMesh = meshes[resolutionIndex];
        });
    }
    #endregion

    public float GetHeight(Vector3 unit)
    {
        return (1 + ridgedNoise.Eval(unit) + simplexNoise.Eval(unit)) * this.radius;
    }

    public void SetLOD(float percent)
    {
        // 0 for minimal, 1 for middle, 2 for maximal resolutions
        // used to index appropriate elements in meshes[], meshesInitalized[] and colliderRadiuses[]
        int resolutionIndex;

        if (percent > lodHighTreshold)
        {
            resolution = maxResolution;
            resolutionIndex = 2;
        }
        else if (percent < lodLowTreshold)
        {
            resolution = minResolution;
            resolutionIndex = 0;
        }
        else
        {
            resolution = middleResolution;
            resolutionIndex = 1;
        }

        // only for current chunk and maybe its neighbours, depends on MapGenerator::MinViewDistance
        colliderEnabled = Mathf.Approximately(percent, 1f);

        // passed via args and not as class fields to copy them
        // in stack and have an unique and unchanged copy
        Generate(resolutionIndex, resolution, false);
    }

    // public void OnDrawGizmosSelected()
    // {
    //     Gizmos.DrawWireCube(region.transform.position, region.bounds.size);
    // }
}

}
