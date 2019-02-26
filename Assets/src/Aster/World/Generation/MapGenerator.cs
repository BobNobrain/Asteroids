using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aster.World.Generation {

public class MapGenerator: MonoBehaviour
{
    public float chunkSize = 30f;

    void Awake()
    {
        GenerateChunk(new Vector3Int(0, 0, 0));
    }

    public void GenerateChunk(Vector3Int chunkCoords)
    {
        Vector3 chunkCenter = ((Vector3) chunkCoords) * chunkSize;
    }
}

}

