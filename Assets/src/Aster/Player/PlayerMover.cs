using Aster.Utils;

namespace Aster.Player
{

public class PlayerMover: PlayerControllerPart
{
    protected FocusableInput input;

    public PlayerMover(PlayerController c, FocusableInput input): base(c)
    {
        this.input = input;
    }

    public virtual void Clear() {}
}

}
