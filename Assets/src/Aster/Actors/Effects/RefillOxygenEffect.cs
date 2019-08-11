namespace Aster.Actors.Effects
{

public class RefillOxygenEffect: Effect
{
    private float amount;
    public RefillOxygenEffect(float amount)
    {
        this.amount = amount;
    }

    public override void Apply(Player.PlayerStats to)
    {
        to.Oxygen.Fill(amount);
    }
}

}
