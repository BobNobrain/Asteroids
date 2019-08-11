using UnityEngine;
using Aster.Player.Inventory;

public class InventoryTest : MonoBehaviour
{
    public InventoryItemType toSpawn;

    void Start()
    {
        var item = toSpawn.Create();
        item.Spawn(transform, new Vector3(0, 0, 10), Quaternion.identity);
    }
}
