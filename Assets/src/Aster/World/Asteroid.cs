﻿using UnityEngine;
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

    [HideInInspector] public ParticleSystem splashEffect;
    public int ParticlesToEmit = 100;

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
        collider = GetComponent<SphereCollider>();
        body = GetComponent<Rigidbody>();

        filter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        filter.sharedMesh = mesh;

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
    public void Generate(bool regenerateMesh)
    {
        var cubeData = regenerateMesh ? meshGenerator.GenerateCube(resolution) : null;

        Dispatcher.InvokeAsync(() => {
            if (this == null) return;

            if (colliderEnabled == collider.isTrigger) collider.isTrigger = !colliderEnabled;

            if (cubeData != null)
            {
                if (colliderEnabled)
                {
                    float minh = cubeData.minmax.x, maxh = cubeData.minmax.y;
                    collider.radius = minh + (maxh - minh) * .9f;
                }

                mesh.Clear();
                mesh.vertices = cubeData.vertices;
                mesh.triangles = cubeData.triangles;
                mesh.uv = cubeData.uvs;

                mesh.RecalculateNormals();
                mesh.RecalculateTangents();
            }
        });
    }
    #endregion

    public float GetHeight(Vector3 unit)
    {
        return (1 + ridgedNoise.Eval(unit) + simplexNoise.Eval(unit)) * this.radius;
    }

    public void SetLOD(float percent)
    {
        int newResolution;

        if (percent > lodHighTreshold)
            newResolution = maxResolution;
        else if (percent < lodLowTreshold)
            newResolution = minResolution;
        else
            newResolution = middleResolution;

        bool regenerateMesh = resolution != newResolution;
        resolution = newResolution;
        // only for current chunk and maybe its neighbours, depends on MapGenerator::MinViewDistance
        colliderEnabled = Mathf.Approximately(percent, 1f);

        Generate(regenerateMesh);
    }

    #region Effects
    void OnCollisionEnter(Collision c)
    {
        if (splashEffect == null) return;

        if (c.relativeVelocity.magnitude > .01f)
        {
            foreach (ContactPoint contact in c.contacts)
            {
                splashEffect.transform.position = contact.point;
                splashEffect.Emit(ParticlesToEmit);
            }
        }
    }
    #endregion

    // public void OnDrawGizmosSelected()
    // {
    //     Gizmos.DrawWireCube(region.transform.position, region.bounds.size);
    // }
}

}
