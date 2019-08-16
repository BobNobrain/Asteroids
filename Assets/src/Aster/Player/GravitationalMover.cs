using UnityEngine;
using Aster.Utils;

namespace Aster.Player
{

public class GravitationalMover: PlayerMover
{
    Settings settings;

    public GravitationalMover(
        Settings settings,
        PlayerController controller,
        FocusableInput input
    ): base(controller, input)
    {
        this.settings = settings;
    }

    // TODO: implement player movement when affected by gravity

    [System.Serializable]
    public class Settings
    {}
}

}
