using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aster.World.Generation {

public class ChunkGenerator
{
    private Transform root;
    private float chunkSize;

    public ChunkGenerator(Transform root, float size)
    {
        this.root = root;
        chunkSize = size;
    }

    public void Generate(ChunkType type, Vector3 center)
    {
        int n = GetRandomAsteroidsCount(type);
        var b = new Bounds(center, chunkSize);

        // TODO: create a new gameobject attached to root and add a chunk component on it

        for (int i = 0; i < n; i++)
        {
            // TODO: use Bounds::RandomXY to place asteroids randomly in chunk space
            // TODO: pick up a random asteroid type and use AsteroidGenerator to generate an asteroid at position
        }
    }


    private int GetRandomAsteroidsCount(ChunkType type)
    {
        int d = (int) ((type.MaxAsteroids - type.MinAsteroids) * Random.value);
        return type.MinAsteroids + d;
    }


    private class Bounds
    {
        public float minx, maxx;
        public float miny, maxy;
        public float minz, maxz;

        public Bounds(Vector3 center, float size)
        {
            float half = size / 2;

            minx = center.x - half;
            maxx = center.x + half;

            miny = center.y - half;
            maxy = center.y + half;

            minz = center.z - half;
            maxz = center.z + half;
        }

        public float PercentX(int i, int max)
        {
            return minx + i * (maxx - minx) / max;
        }

        public Vector3 RandomYZ(float x)
        {
            return new Vector3(x, Random.Range(miny, maxy), Random.Range(minz, maxz));
        }
    }
}

}
