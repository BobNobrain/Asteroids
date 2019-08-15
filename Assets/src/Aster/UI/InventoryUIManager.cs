using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Aster.Actors.Inventory;

namespace Aster.UI
{

[System.Serializable]
public class InventoryUIManager: UISubManager
{
    public RectTransform contentParent;
    public GameObject contentPanel;
    public GameObject infoPanel;

    public List<InventoryItemElement> elements;

    public Text selectionName;
    public Text selectionDescription;
    public Text massLabel;

    public GameObject itemElementPrefab;

    public GameObject emptyPlank;
    public GameObject itemsScrollView;

    private int selectionIndex = 0;
    private Inventory model;

    public void SetVisibility(bool visible)
    {
        contentPanel.SetActive(visible);
        infoPanel.SetActive(visible);
    }

    public void OnInventoryChanged(Inventory inv)
    {
        model = inv;

        if (inv.Count == 0)
        {
            emptyPlank.SetActive(true);
            itemsScrollView.SetActive(false);
        }
        else
        {
            emptyPlank.SetActive(false);
            itemsScrollView.SetActive(true);

            int i = 0;
            for (; i < inv.Count; i++)
            {
                if (i == elements.Count)
                {
                    // spawn a new element
                    var obj = GameObject.Instantiate(
                        itemElementPrefab,
                        Vector3.zero,
                        Quaternion.identity,
                        contentParent
                    );
                    elements.Add(obj.GetComponent<InventoryItemElement>());
                }

                var el = elements[i];
                el.gameObject.SetActive(true);
                el.item = inv[i];
                el.UpdateWithItem();
            }
            for (; i < elements.Count; i++)
            {
                var el = elements[i];
                el.gameObject.SetActive(false);
                el.item = null;
            }
        }
    }

    public void ToggleVisibility()
    {
        SetVisibility(!contentPanel.activeSelf);
    }
}

}
