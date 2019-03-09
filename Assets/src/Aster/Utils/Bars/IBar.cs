using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aster.Utils.Bars {

public interface IBar
{
    float Value { get; }
    bool IsFull { get; }
    bool IsEmpty { get; }
    bool Acquire(float amount);
    void Fill(float amount);

}

public delegate void BarEmptyListener();
public delegate void BarChangedListener(float amount);
public delegate void BarFullListener();

}
