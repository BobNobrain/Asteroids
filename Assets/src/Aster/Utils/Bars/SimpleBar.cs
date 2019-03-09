using UnityEngine;

namespace Aster.Utils.Bars {

[System.Serializable]
public class SimpleBar: IBar
{
    protected BarEmptyListener emptyListener;
    protected BarChangedListener changedListener;
    protected BarFullListener fullListener;

    [SerializeField]
    [Range(0f, 1f)]
    protected float v;

    public float Value
    {
        get { return v; }
    }

    public virtual bool Acquire(float amount)
    {
        if (changedListener != null) changedListener(-amount);
        if (amount > v)
        {
            if (emptyListener != null) emptyListener();
            return false;
        }
        v -= amount;
        return true;
    }
    public virtual void Fill(float amount)
    {
        if (changedListener != null) changedListener(amount);
        v += amount;
        if (v > 1f)
        {
            if (fullListener != null) fullListener();
            v = 1f;
        }
    }

    public bool IsFull
    {
        get { return Mathf.Approximately(v, 1f); }
    }
    public bool IsEmpty
    {
        get { return Mathf.Approximately(v, 0f); }
    }
}

}
