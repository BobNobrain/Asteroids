using UnityEngine;
using Aster.World.Generation;

namespace Aster.World {

[RequireComponent(typeof(BoxCollider))]
public class Chunk: MonoBehaviour, ILODController {
    private CompoundLODController asteroids;
    private BoxCollider bounds;

    private MapGenerator root;

    public Vector3Int Coords;

    public void Init(float chunkSize, MapGenerator generator, Vector3Int coords)
    {
        this.Coords = coords;
        asteroids = new CompoundLODController();
        bounds = GetComponent<BoxCollider>();
        bounds.size = new Vector3(chunkSize, chunkSize, chunkSize);
        root = generator;
    }

    public void AttachAsteroid(Asteroid a)
    {
        asteroids.children.Add(a);
    }

    public void SetLOD(float percent)
    {
        if (Mathf.Approximately(percent, 0))
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            asteroids.SetLOD(percent);
        }
    }

    public void OnPlayerEnter()
    {
        root.UpdateCenter(this);
    }

    public void Dispose()
    {
        // TODO: save chunk state to disk
        Destroy(gameObject);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, bounds.size);
    }
}

}
