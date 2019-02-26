using UnityEngine;

namespace Aster {
namespace World {

public class AsteroidType: ScriptableObject {
    public GameObject asteroidPrefab;
    public float MinRadius;
    public float MaxRadius;
    public Material[] Materials;
    public float Density;

    public float Probability;
}

}}
