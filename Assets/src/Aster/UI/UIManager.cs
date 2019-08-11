using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Aster.UI
{

public class UISubManager
{
    public UIManager ui;

    public virtual void Init(UIManager manager)
    {
        ui = manager;
    }
}

public class UIManager: MonoBehaviour
{
    public CrosshairManager crosshair;
    public InventoryUIManager inventory;

    private void Awake()
    {
        crosshair.Init(this);
        inventory.Init(this);

        crosshair.SetType(CrosshairType.DEFAULT);
        inventory.SetVisibility(false);
    }
}

}
