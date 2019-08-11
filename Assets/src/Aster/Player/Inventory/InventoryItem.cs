using UnityEngine;
using Aster.Actors.Effects;
using Aster.Objects;

namespace Aster.Player.Inventory
{

[System.Serializable]
public class InventoryItem
{
    public InventoryItemType type { get; private set; }

    public InventoryItem(InventoryItemType t)
    {
        type = t;
    }

    public virtual Effect Apply(Inventory container, int selfIndex)
    { return null; }

    public GameObject Spawn(Transform parent, Vector3 position, Quaternion rotation)
    {
        var result = GameObject.Instantiate(type.itemPrefab, position, rotation, parent);
        var gatherable = result.GetComponent<GatherableObject>();
        gatherable.gatherableItem = this;

        result.GetComponent<Rigidbody>().mass = type.mass;

        return result;
    }
}

}

