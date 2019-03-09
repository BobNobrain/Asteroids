using System.Collections.Generic;
using UnityEngine;
using UnityToolbag;
using Aster.Utils;

namespace Aster.World.Generation {

public class MapGenerator: MonoBehaviour
{
    public int seed = 0;
    public float chunkSize = 70f;

    [Range(1, 15)]
    public int MaxViewDistance = 3;
    [Range(1, 10)]
    public int MinViewDistance = 1;

    public ChunkType[] chunkTypes;

    public GameObject chunkPrefab;

    private ChunkGenerator generator;
    private Chunk center;
    private List<Chunk> activeChunks;

    public object _lock = new object();
    public volatile int ProcessingChunksCount = 0;

    public int TotalChunks { get; private set; }


    void Start()
    {
        Random.InitState(seed);
        generator = new ChunkGenerator(this, chunkSize);

        int cubeSide = MaxViewDistance * 2 + 1;
        TotalChunks = cubeSide * cubeSide * cubeSide;
        activeChunks = new List<Chunk>(TotalChunks);

        GenerateZeroNeighborhood();
    }

    public void GenerateZeroNeighborhood()
    {
        var zero = GenerateChunk(new Vector3Int(0, 0, 0));
        activeChunks.Add(zero);

        center = zero;
        GenerateNewChunks(
            new IntCube(Vector3Int.zero, 0),
            new IntCube(Vector3Int.zero, MaxViewDistance)
        );
        UpdateLODs();
    }

    public Chunk GenerateChunk(Vector3Int chunkCoords)
    {
        var chunkType = Rnd.Pick(chunkTypes);
        return generator.Generate(chunkType, chunkCoords, chunkPrefab);
    }

    public void UpdateCenter(Chunk newCenter)
    {
        if (center == newCenter) return;
        var oldCenter = center;
        center = newCenter;
        ClearFarChunks();
        GenerateNewChunks(
            new IntCube(oldCenter.Coords, MaxViewDistance),
            new IntCube(newCenter.Coords, MaxViewDistance)
        );
        UpdateLODs();
    }

    private void ClearFarChunks()
    {
        int totalDeleted = 0;
        for (int i = 0; i < activeChunks.Count; i++)
        {
            var chunk = activeChunks[i];
            if (!IntCube.WithinCube(center.Coords, MaxViewDistance, chunk.Coords))
            {
                activeChunks.RemoveAt(i);
                chunk.Dispose();
                --i;
                ++totalDeleted;
            }
        }
        Debug.Log("Removing far chunks: " + totalDeleted.ToString());
    }
    private void GenerateNewChunks(IntCube oldArea, IntCube newArea)
    {
        var diff = newArea.Diff(oldArea);
        foreach (var coords in diff)
        {
            var chunk = GenerateChunk(coords);
            activeChunks.Add(chunk);
        }
        Debug.Log("Generated new chunks: " + diff.Count);
        lock (_lock)
        {
            ProcessingChunksCount = diff.Count;
            // Debug.Log("Set to" + ProcessingChunksCount);
        }
    }
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
                lock (_lock)
                {
                    ProcessingChunksCount -= 1;
                    // Debug.Log("-- =>" + ProcessingChunksCount);
                }
                return null;
            });
            f.OnError(future => {
                Debug.LogError(future.error);
                lock (_lock)
                {
                    ProcessingChunksCount -= 1;
                    // Debug.Log("-- =>" + ProcessingChunksCount);
                }
            });
        }
    }
}

}

