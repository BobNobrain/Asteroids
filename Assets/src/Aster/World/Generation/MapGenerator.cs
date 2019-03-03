using System.Collections.Generic;
using UnityEngine;
using Aster.Utils;

namespace Aster.World.Generation {

public class MapGenerator: MonoBehaviour
{
    public int seed = 0;
    public float chunkSize = 70f;

    [Range(1, 10)]
    public int MaxViewDistance = 3;

    public ChunkType[] chunkTypes;

    public GameObject chunkPrefab;

    private ChunkGenerator generator;
    private Chunk center;
    private List<Chunk> activeChunks;

    void Awake()
    {
        Random.InitState(seed);
        generator = new ChunkGenerator(this, chunkSize);

        int cubeSide = MaxViewDistance * 2 + 1;
        activeChunks = new List<Chunk>(cubeSide * cubeSide * cubeSide);

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
    }
    private void UpdateLODs()
    {
        float mvd = (float) (MaxViewDistance + 1);
        foreach (var chunk in activeChunks)
        {
            int d = Metrics.DiamondDistance(chunk.Coords, center.Coords);
            if (d > MaxViewDistance)
            {
                chunk.SetLOD(0);
            }
            else
            {
                float linearLod = (MaxViewDistance + 1 - d) / mvd;
                float lod = linearLod * linearLod;
                if (lod > 1f) lod = 1f;
                chunk.SetLOD(lod);
            }
        }
    }
}

}

