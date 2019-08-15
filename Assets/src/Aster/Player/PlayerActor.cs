using Aster.Actors;

namespace Aster.Player
{

public class PlayerActor: Actor
{
    private PlayerController player;
    private float originalPlayerMass;

    public PlayerActor(PlayerController controller)
    {
        player = controller;
        originalPlayerMass = player.body.mass;

        Inventory = new Actors.Inventory.Inventory();
        Inventory.ContentChanged += UpdatePlayerMass;
    }

    public void UpdatePlayerMass(Actors.Inventory.Inventory inv)
    {
        player.body.mass = originalPlayerMass + inv.Mass;
    }
}

}
