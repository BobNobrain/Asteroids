using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aster.Player
{

public class GravitationalMover: PlayerControllerPart
{
    Settings settings;

    public GravitationalMover(Settings settings, PlayerController controller): base(controller)
    {
        this.settings = settings;
    }

    // TODO: implement player movement when affected by gravity

    [System.Serializable]
    public class Settings
    {}
}

}
