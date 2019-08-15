using UnityEngine;
using Aster.Actors.Inventory;

namespace Aster.Objects
{

[RequireComponent(typeof(Rigidbody))]
public class GatherableObject: InteractableObject
{
    public InventoryItem gatherableItem;

    private void Start()
    {
        GetComponent<Rigidbody>().AddTorque(Random.onUnitSphere);
    }

    public override void Interact(Actors.Actor a)
    {
        if (a.Inventory != null)
        {
            a.Inventory.Add(gatherableItem);
            // despawn
            Destroy(gameObject);
        }
    }

    public override bool CanInteract(Actors.Actor a)
    {
        return a.Inventory != null;
    }

    public override string Name
    {
        get
        {
            return gatherableItem.type.name;
        }
    }
}

}
