using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aster.Utils {

public static class Rnd {
    public static T Pick<T>(T[] from)
    {
        float r = Random.value;
        int i = (int) (from.Length * r);
        if (i == from.Length) i = 0;
        return from[i];
    }

    public static T WeightedPick<T>(IWeightedItem<T>[] from)
    {
        float sum = 0;
        for (int i = 0; i < from.Length; i++)
        {
            sum += from[i].Weight;
        }

        float r = Random.value * sum;
        for (int i = 0; i < from.Length; i++)
        {
            float p = from[i].Weight;
            if (p >= r)
            {
                return from[i].Item;
            }

            r -= p;
        }

        Debug.LogWarning("WeightedPick calculation error");
        return from[from.Length - 1].Item;
    }
}

}
