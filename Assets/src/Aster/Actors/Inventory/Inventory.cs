using System.Collections.Generic;

namespace Aster.Actors.Inventory
{

public class Inventory
{
    protected List<InventoryItem> content;
    public object Owner { get; private set; }

    public Inventory()
    {
        content = new List<InventoryItem>();
        Mass = 0;
    }

    public int Count
    {
        get { return content.Count; }
    }

    public InventoryItem this[int i]
    {
        get { return content[i]; }
        set
        {
            if (content[i] != value)
            {
                content[i] = value;
                if (ContentChanged != null) { ContentChanged.Invoke(this); }
            }
        }
    }

    #region Mass
    private float getActualMass()
    {
        float total = 0;
        foreach(var entry in content)
        {
            total += entry.type.mass;
        }
        return total;
    }

    public float Mass { get; protected set; }
    #endregion

    #region Modification
    public void Clear()
    {
        content.Clear();
        Mass = 0;
        if (ContentChanged != null) { ContentChanged.Invoke(this); }
    }

    public void Add(InventoryItem item)
    {
        content.Add(item);
        Mass += item.type.mass;
        if (ContentChanged != null) { ContentChanged.Invoke(this); }
    }

    public void RemoveAt(int index)
    {
        Mass -= content[index].type.mass;
        content.RemoveAt(index);
        if (ContentChanged != null) { ContentChanged.Invoke(this); }
    }

    public void ApplyAt(int index, Player.PlayerStats applyTo)
    {
        var effect = content[index].Apply(this, index);
        if (effect != null) { effect.Apply(applyTo); }
        if (ContentChanged != null) { ContentChanged.Invoke(this); }
    }
    #endregion

    #region Listeners
    public delegate void ContentChangeListener(Inventory target);
    public event ContentChangeListener ContentChanged;
    #endregion
}

}
