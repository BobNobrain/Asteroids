using UnityEngine;
using Noise;
using Aster.Utils;

namespace Aster.World {

public class AsteroidGenerator
{
    public static Asteroid Generate(Vector3 position, AsteroidType type, GameObject parentChunk)
    {
        var asteroidObj = GameObject.Instantiate(
            type.asteroidPrefab,
            position,
            Random.rotation,
            parentChunk.transform
        );

        var asteroid = asteroidObj.GetComponent<Asteroid>();

        asteroid.radius = Random.Range(type.MinRadius, type.MaxRadius);
        asteroid.material = Rnd.Pick(type.Materials);
        asteroid.density = type.Density;
        asteroid.seed = (int) Random.Range(0, 65535);
        asteroid.rotationSpeed = Random.Range(0.01f, 0.5f);

        asteroid.Init();

        return asteroid;
    }
}

}
