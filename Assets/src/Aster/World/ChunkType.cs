using UnityEngine;
using Aster.Utils;

namespace Aster.World {

[CreateAssetMenu(menuName = "Aster/Chunk Type")]
public class ChunkType: ScriptableObject
{
    public AsteroidProbability[] availableTypes;
    public int MinAsteroids;
    public int MaxAsteroids;

    [System.Serializable]
    public class AsteroidProbability: IWeightedItem<AsteroidType>
    {
        public AsteroidType type;
        public float probability;

        public float Weight { get { return probability; } }
        public AsteroidType Item { get { return type; } }
    }
}

}
