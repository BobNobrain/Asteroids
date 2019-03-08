using UnityEngine;
using Aster.Utils;

namespace Aster.World.Generation {

public class ChunkGenerator
{
    private Transform root;
    private float chunkSize;
    private MapGenerator g;

    public ChunkGenerator(MapGenerator generator, float size)
    {
        root = generator.transform;
        chunkSize = size;
        g = generator;
    }

    public Chunk Generate(ChunkType type, Vector3Int coords, GameObject chunkPrefab)
    {
        Vector3 center = ((Vector3) coords) * chunkSize;
        int n = GetRandomAsteroidsCount(type);
        var b = new Bounds(center, chunkSize);

        var chunkObj = GameObject.Instantiate(chunkPrefab, center, Quaternion.identity, root);
        var chunk = chunkObj.GetComponent<Chunk>();
        chunk.Init(chunkSize, g, coords);

        for (int i = 0; i < n; i++)
        {
            float x = b.PercentX(i, n);
            Vector3 place = b.RandomYZ(x);

            var asteroidType = Rnd.WeightedPick(type.availableTypes);

            var asteroid = AsteroidGenerator.Generate(place, asteroidType, chunkObj);
            // TODO: refactor!
            asteroid.splashEffect = g.asteroidSplashEffect;
            chunk.AttachAsteroid(asteroid);
        }

        return chunk;
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
