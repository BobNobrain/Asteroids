using System.Collections.Generic;
using UnityEngine;
using Aster.World.Generation;

namespace Aster.World {

[RequireComponent(typeof(BoxCollider))]
public class Chunk: MonoBehaviour, ILODController {
    private List<Asteroid> asteroids;
    public float lod = 0f;
    private BoxCollider bounds;

    private MapGenerator root;

    public Vector3Int Coords;

    public void Init(float chunkSize, MapGenerator generator, Vector3Int coords)
    {
        this.Coords = coords;
        asteroids = new List<Asteroid>();
        bounds = GetComponent<BoxCollider>();
        bounds.size = new Vector3(chunkSize, chunkSize, chunkSize);
        root = generator;
    }

    public void AttachAsteroid(Asteroid a)
    {
        asteroids.Add(a);
    }

    public void SetLOD(float percent)
    {
        lod = percent;
        if (Mathf.Approximately(percent, 0))
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);

            StartCoroutine(UpdateAsteroidLods());
        }
    }

    private System.Collections.IEnumerator UpdateAsteroidLods()
    {
        foreach (var a in asteroids)
        {
            a.SetLOD(lod);
            yield return null;
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
