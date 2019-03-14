using UnityEngine;

namespace Aster.Tools.Weapons {

public interface IWeapon
{
    /// <summary>
    /// Performs primary fire shoot (e.g. when LMB)
    /// </summary>
    void PrimaryTrigger();

    /// <summary>
    /// Performs secondary fire shoot (e.g. when RMB)
    /// </summary>
    void SecondaryTrigger();
}

}

