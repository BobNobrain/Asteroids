using UnityEngine;
using Aster.World;

namespace Aster.Player {

public class PlayerTracker: MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var chunk = other.GetComponent<Chunk>();
        if (chunk != null)
        {
            chunk.OnPlayerEnter();
        }
    }
}

}
