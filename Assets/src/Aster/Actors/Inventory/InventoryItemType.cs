using UnityEngine;

namespace Aster.Actors.Inventory
{

[CreateAssetMenu(menuName = "Aster/Inventory Item")]
public class InventoryItemType: ScriptableObject
{
    public string itemName = "[_]";
    [TextArea] public string itemDescription = "";
    public float mass = 0.001f; // kg
    public Sprite displayIcon;
    public GameObject itemPrefab;

    public virtual InventoryItem Create()
    {
        return new InventoryItem(this);
    }
}

}

