using UnityEngine;

namespace Aster.World {

[RequireComponent(typeof(BoxCollider))]
public class Chunk: MonoBehaviour, ILODController {
    private CompoundLODController asteroids;
    private BoxCollider bounds;

    public void Init(float chunkSize)
    {
        asteroids = new CompoundLODController();
        bounds = GetComponent<BoxCollider>();
        bounds.size = new Vector3(chunkSize, chunkSize, chunkSize);
    }

    public void AttachAsteroid(Asteroid a)
    {
        asteroids.children.Add(a);
    }

    public void SetLOD(float percent)
    {
        asteroids.SetLOD(percent);
    }
}

}
