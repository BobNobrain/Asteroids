using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Aster.Actors.Inventory;
using Aster.Utils;

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

    private FocusableInput input = new FocusableInput();

    public override void Init(UIManager manager)
    {
        base.Init(manager);
        contentPanel.SetActive(false);
        infoPanel.SetActive(false);
    }

    public void SetVisibility(bool visible, FocusableInput callerInput)
    {
        contentPanel.SetActive(visible);
        infoPanel.SetActive(visible);

        if (visible)
        {
            callerInput.TransferFocus(this.input);
        }
        else
        {
            this.input.TransferFocus(callerInput);
        }
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
                        contentParent,
                        false
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

    public void ToggleVisibility(FocusableInput input)
    {
        SetVisibility(!contentPanel.activeSelf, input);
    }
}

}
