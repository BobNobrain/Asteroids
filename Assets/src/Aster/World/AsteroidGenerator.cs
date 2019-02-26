using Noise;
using UnityEngine;

namespace Aster { namespace World {

public class AsteroidGenerator: MonoBehaviour
{
    public GameObject asteroidPrefab;
    public Material[] materials;
    public int seed = 0;
    [Range(1, 50)]
    public int numAsteroids = 20;

    public void Awake()
    {
        Random.InitState(seed);

        float gap = 5f;
        float boxSizeHalf = numAsteroids * gap / 2;
        for (int i = 0; i < numAsteroids; i++)
        {
            float randomSize = Random.Range(0.5f, 2.5f);
            Vector3 randomPosition = new Vector3(
                Random.Range(-boxSizeHalf, boxSizeHalf),
                Random.Range(-boxSizeHalf, boxSizeHalf),
                -boxSizeHalf + i * gap
            );
            var next = Instantiate(asteroidPrefab, randomPosition, Random.rotation, transform);
            var asteroid = next.GetComponent<Asteroid>();
            asteroid.radius = randomSize;
            asteroid.seed = seed + i;
            asteroid.rotationSpeed = Random.Range(0.01f, 0.5f);

            int materialIndex = (int) (Random.value * materials.Length);
            if (materialIndex == materials.Length)
            {
                materialIndex = 0;
            }
            asteroid.material = materials[materialIndex];
            asteroid.Generate();
        }
    }
}

}}
