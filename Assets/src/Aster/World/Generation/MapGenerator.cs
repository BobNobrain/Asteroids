using System.Collections.Generic;
using UnityEngine;
using UnityToolbag;
using Aster.Utils;

namespace Aster.World.Generation {

public class MapGenerator: MonoBehaviour
{
    public int seed = 0;
    public float chunkSize = 70f;

    /// <summary>
    /// Maximal view distance (rendered with minimal possible LoD)
    /// </summary>
    [Range(1, 15)]
    public int MaxViewDistance = 3;

    /// <summary>
    /// Maximal distance where LoD is kept at maximal level
    /// </summary>
    [Range(1, 10)]
    public int MinViewDistance = 1;

    /// <summary>
    /// Maximal distance at which chunks are still kept in memory
    /// </summary>
    [Range(3, 15)]
    public int MaxLoadedDistance = 5;

    public ChunkType[] chunkTypes;

    public GameObject chunkPrefab;

    public Transform ChunksRoot;
    public Transform AstersRoot;

    private ChunkGenerator generator;

    /// <summary>
    /// Chunk that contains player at the moment
    /// </summary>
    private Chunk center;

    /// <summary>
    /// All loaded chunks
    /// </summary>
    private List<Chunk> activeChunks;

    /// <summary>
    /// Center of area that is currently loaded into memory
    /// </summary>
    private Vector3Int loadedCenter;

    // public object _lock = new object();
    public int ProcessingChunksCount { get; private set; }
    public void NotifyRendered()
    {
        if (Dispatcher.isMainThread)
        {
            if (ProcessingChunksCount > 0)
            {
                --ProcessingChunksCount;
            }
        }
        else
        {
            throw new System.InvalidOperationException("NotifyRendered should be called from main thread");
        }
    }

    public int TotalChunks { get; private set; }


    void Start()
    {
        Random.InitState(seed);
        generator = new ChunkGenerator(this, chunkSize);

        int cubeSide = MaxLoadedDistance * 2 + 1;
        TotalChunks = cubeSide * cubeSide * cubeSide;
        activeChunks = new List<Chunk>(TotalChunks);

        GenerateZeroNeighborhood();
    }

    public void GenerateZeroNeighborhood()
    {
        loadedCenter = new Vector3Int(0, 0, 0);
        var zero = GenerateChunk(loadedCenter);
        activeChunks.Add(zero);

        center = zero;
        GenerateNewChunks(
            new IntCube(Vector3Int.zero, 0),
            new IntCube(Vector3Int.zero, MaxLoadedDistance)
        );
        UpdateLODs();
    }

    public Chunk GenerateChunk(Vector3Int chunkCoords)
    {
        var chunkType = Rnd.Pick(chunkTypes);
        return generator.Generate(chunkType, chunkCoords, chunkPrefab);
    }

    /// <summary>
    /// Updates central chunk to a new chunk, regenerating LoDs and loaded chunks appropriately
    /// </summary>
    /// <param name="newCenter">New center chunk</param>
    public void UpdateCenter(Chunk newCenter)
    {
        if (center == newCenter) return;
        var oldCenter = center;
        center = newCenter;

        if (!IntCube.WithinCube(
            loadedCenter,
            MaxLoadedDistance,
            newCenter.Coords,
            MaxViewDistance
        ))
        {
            // Player needs to render chunks that were not yet loaded
            // Move the IntCube of loaded chunks to fit the player

            ClearFarChunks();
            GenerateNewChunks(
                new IntCube(loadedCenter, MaxLoadedDistance),
                new IntCube(newCenter.Coords, MaxLoadedDistance)
            );

            loadedCenter = newCenter.Coords;
        }

        UpdateLODs();
    }

    /// <summary>
    /// Disposes chunks that are too far from player
    /// (farther then MaxLoadedDistance)
    /// </summary>
    private void ClearFarChunks()
    {
        int totalDeleted = 0;
        for (int i = 0; i < activeChunks.Count; i++)
        {
            var chunk = activeChunks[i];
            if (!IntCube.WithinCube(center.Coords, MaxLoadedDistance, chunk.Coords))
            {
                activeChunks.RemoveAt(i);
                chunk.Dispose();
                --i;
                ++totalDeleted;
            }
        }
        Debug.Log("Removing far chunks: " + totalDeleted.ToString());
    }

    /// <summary>
    /// Generates new chunks that will complete oldArea to newArea
    /// </summary>
    /// <param name="oldArea">Already existing chunks</param>
    /// <param name="newArea">Area that should be filled with chunks</param>
    private void GenerateNewChunks(IntCube oldArea, IntCube newArea)
    {
        var diff = newArea.Diff(oldArea);
        foreach (var coords in diff)
        {
            var chunk = GenerateChunk(coords);
            activeChunks.Add(chunk);
        }
        Debug.Log("Generated new chunks: " + diff.Count);
        ProcessingChunksCount = diff.Count;
    }

    /// <summary>
    /// Updates LoDs of all loaded chunks (uses ThreadPool)
    /// </summary>
    private void UpdateLODs()
    {
        float mvd = (float) (MaxViewDistance - MinViewDistance + 1);
        foreach (var chunk in activeChunks)
        {
            int d = Metrics.DiamondDistance(chunk.Coords, center.Coords);
            float lod;
            if (d > MaxViewDistance)
            {
                lod = 0f;
            }
            else if (d <= MinViewDistance)
            {
                lod = 1f;
            }
            else
            {
                float linearLod = (MaxViewDistance + 1 - d) / mvd;
                lod = linearLod * linearLod;
                if (lod > 1f) lod = 1f;
            }

            var f = new Future<Object>();
            f.Process(() => {
                chunk.SetLOD(lod);
                return null;
            });
            f.OnError(future => {
                Debug.LogError(future.error);
                ProcessingChunksCount -= 1;
            });
        }
    }
}

}

