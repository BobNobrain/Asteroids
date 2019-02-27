using System.Collections.Generic;
using UnityEngine;

namespace Aster.World {

public class CompoundLODController: ILODController
{
    public List<ILODController> children;

    public CompoundLODController()
    {
        children = new List<ILODController>();
    }

    public void SetLOD(float percent)
    {
        foreach (var child in children)
        {
            child.SetLOD(percent);
        }
    }
}

}
