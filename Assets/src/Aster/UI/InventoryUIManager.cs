using UnityEngine;
using UnityEngine.UI;
using Aster.Player.Inventory;

namespace Aster.UI
{

[System.Serializable]
public class InventoryUIManager: UISubManager
{
    public RectTransform contentParent;
    public GameObject contentPanel;
    public GameObject infoPanel;

    public Text selectionName;
    public Text selectionDescription;
    public Text massLabel;

    public GameObject itemElementPrefab;

    public void SetVisibility(bool visible)
    {
        contentPanel.SetActive(visible);
        infoPanel.SetActive(visible);
    }

    public void OnInventoryChanged(Inventory inv) {}
}

}
