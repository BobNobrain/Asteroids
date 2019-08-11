using Aster.Actors.Effects;

namespace Aster.Player.Inventory
{

[System.Serializable]
public class OxygenRefillerItem: InventoryItem
{
    public float refillAmount;

    public OxygenRefillerItem(OxygenRefillerType t, float amount): base(t)
    {
        refillAmount = amount;
    }

    public override Effect Apply(Inventory container, int selfIndex)
    {
        container.RemoveAt(selfIndex);
        return new RefillOxygenEffect(refillAmount);
    }
}

}

