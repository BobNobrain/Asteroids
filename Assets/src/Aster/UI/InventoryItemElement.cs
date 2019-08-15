using UnityEngine;
using UnityEngine.UI;
using Aster.Actors.Inventory;

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
        elementText.gameObject.SetActive(false);
        elementProgressBar.gameObject.SetActive(false);
        elementImage.sprite = item.type.displayIcon;
    }
}

}
