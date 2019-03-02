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
        for (int i = 0; i < activeChunks.Count; i++)
        {
            var chunk = activeChunks[i];
            if (Diamond.IntegerDistance(chunk.Coords, center.Coords) > MaxViewDistance)
            {
                activeChunks.RemoveAt(i);
                --i;
            }
        }
    }
    private void GenerateNewChunks(IntCube oldArea, IntCube newArea)
    {
        var diff = newArea.Diff(oldArea);
        foreach (var coords in diff)
        {
            var chunk = GenerateChunk(coords);
            activeChunks.Add(chunk);
        }
    }
    private void UpdateLODs()
    {
        float mvd = (float) (MaxViewDistance + 1);
        foreach (var chunk in activeChunks)
        {
            int d = Diamond.IntegerDistance(chunk.Coords, center.Coords);
            Debug.Log("d(" + chunk.Coords.ToString() + ", " + center.Coords.ToString() + ") = " + d.ToString());
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
                Debug.Log("Set LOD of " + lod.ToString() + " for chunk #" + chunk.Coords.ToString());
            }
        }
    }
}

}

