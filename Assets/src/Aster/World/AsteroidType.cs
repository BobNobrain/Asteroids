using UnityEngine;

namespace Aster {
namespace World {

[CreateAssetMenu(menuName = "Aster/Asteroid Type")]
public class AsteroidType: ScriptableObject {
    public GameObject asteroidPrefab;
    public float MinRadius;
    public float MaxRadius;
    public Material[] Materials;
    public float Density;
}

}}
