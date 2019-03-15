using UnityEngine;

namespace Aster.Tools.Weapons {

public interface IWeapon
{
    /// <summary>
    /// Performs weapon initialization after it was instantiated
    /// </summary>
    /// <param name="playerObject">GameObject that represents player</param>
    /// <param name="bulletsRoot">GameObject to use as parent for instantiating bullets</param>
    void Init(GameObject playerObject, GameObject bulletsRoot);

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

