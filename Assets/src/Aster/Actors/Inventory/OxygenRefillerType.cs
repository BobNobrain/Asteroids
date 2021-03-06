﻿using UnityEngine;

namespace Aster.Actors.Inventory
{

[CreateAssetMenu(menuName = "Aster/Oxygen Refiller")]
public class OxygenRefillerType: InventoryItemType
{
    public float oxygenRefillAmount = 0f;

    public override InventoryItem Create()
    {
        return new OxygenRefillerItem(this, oxygenRefillAmount);
    }
}

}

