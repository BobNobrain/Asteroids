using System.Collections.Generic;
using UnityEngine;
using Aster.Utils;

namespace Aster.World.Generation {

public class MapGenerator: MonoBehaviour
{
    public int seed = 0;
    public float chunkSize = 70f;

    public ChunkType[] chunkTypes;

    public GameObject chunkPrefab;

    private ChunkGenerator generator;
    private List<Chunk> activeChunks;

    void Awake()
    {
        Random.InitState(seed);
        generator = new ChunkGenerator(transform, chunkSize);
        activeChunks = new List<Chunk>();

        var zero = GenerateChunk(new Vector3Int(0, 0, 0));
        activeChunks.Add(zero);
        zero.SetLOD(1f);
    }

    public Chunk GenerateChunk(Vector3Int chunkCoords)
    {
        Vector3 chunkCenter = ((Vector3) chunkCoords) * chunkSize;
        var chunkType = Rnd.Pick(chunkTypes);

        return generator.Generate(chunkType, chunkCenter, chunkPrefab);
    }
}

}

