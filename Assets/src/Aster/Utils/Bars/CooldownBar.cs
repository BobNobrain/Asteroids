using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aster.Utils.Bars {

[System.Serializable]
public class CooldownBar: SimpleBar
{
    private bool cooldown;

    public override bool Acquire(float amount)
    {
        if (cooldown) return false;

        if (changedListener != null) changedListener(-amount);
        if (amount >= v)
        {
            if (emptyListener != null) emptyListener();
            v = 0f;
            cooldown = true;
            return true;
        };
        v -= amount;
        return true;
    }
    public override void Fill(float amount)
    {
        base.Fill(amount);
        if (v >= 1f)
        {
            cooldown = false;
        }
    }

    public bool IsCooldown
    {
        get { return cooldown; }
    }
}

}
