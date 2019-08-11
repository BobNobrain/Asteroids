using UnityEngine;
using UnityEngine.UI;
using Aster.Player.Inventory;

namespace Aster.UI
{

public class InventoryItemElement: MonoBehaviour
{
    public Text elementText;
    public Image elementImage;
    public Slider elementProgressBar;

    public InventoryItem item;

    public void UpdateWithItem()
    {
        elementText.enabled = false;
        elementProgressBar.enabled = false;
        elementImage.sprite = item.type.displayIcon;
    }
}

}
