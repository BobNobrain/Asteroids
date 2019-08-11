using UnityEngine;
using Aster.Player.Inventory;

namespace Aster.Objects
{

[RequireComponent(typeof(Rigidbody))]
public class GatherableObject: MonoBehaviour
{
    public InventoryItem gatherableItem;

    private void Start()
    {
        GetComponent<Rigidbody>().AddTorque(Random.onUnitSphere);
    }
}

}
