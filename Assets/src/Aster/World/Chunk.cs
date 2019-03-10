using System.Collections.Generic;
using UnityEngine;
using UnityToolbag;
using Aster.World.Generation;

namespace Aster.World {

[RequireComponent(typeof(BoxCollider))]
public class Chunk: MonoBehaviour, ILODController {
    private List<Asteroid> asteroids;
    public float lod = 0f;
    private BoxCollider bounds;

    public MapGenerator root;

    public Vector3Int Coords;

    public int guestsLayerMask = 8; // "Chunk Guests"

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
        // Debug.Log("Attaching an asteroid to chunk");
        asteroids.Add(a);
        a.region = this;
    }
    public void DetachAsteroid(Asteroid a)
    {
        Debug.Log("Detaching an asteroid from chunk");
        asteroids.Remove(a);
        a.region = null;
    }

    public void SetLOD(float percent)
    {
        lod = percent;
        foreach (var a in asteroids)
        {
            a.SetLOD(percent);
        }
    }

    #region ChunkGuests
    public void OnPlayerEnter()
    {
        root.UpdateCenter(this);
    }
    public void OnAsteroidEnter(Asteroid entered)
    {
        if (entered.region == this) return;

        entered.region.DetachAsteroid(entered);
        AttachAsteroid(entered);
        entered.SetLOD(lod);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == guestsLayerMask)
        {
            var asteroid = other.gameObject.GetComponent<Asteroid>();
            if (asteroid != null)
            {
                OnAsteroidEnter(asteroid);
            }
            else
            {
                OnPlayerEnter();
            }
        }
    }
    #endregion

    public void Dispose()
    {
        // TODO: save chunk state to disk
        Dispatcher.InvokeAsync(() => {
            foreach (var a in asteroids)
            {
                Destroy(a.gameObject);
            }
            Destroy(gameObject);
        });
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, bounds.size);
    }
}

}
