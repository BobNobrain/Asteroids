using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aster.World {

public class ChunkType: ScriptableObject
{
    public AsteroidProbability[] availableTypes;
    public int MinAsteroids;
    public int MaxAsteroids;

    [System.Serializable]
    public class AsteroidProbability
    {
        public AsteroidType type;
        public float probability;
    }
}

}
