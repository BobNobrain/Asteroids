using UnityEngine;
using UnityEngine.UI;
using Aster.Player;
using Aster.UI.BarControllers;

namespace Aster.UI {

public class PlayerUI: MonoBehaviour
{
    public PlayerStats player;
    public SimpleBarController healthBarController;
    public SimpleBarController oxygenBarController;
    public CooldownBarController staminaBarController;

    void Start()
    {
        healthBarController.Start();
        oxygenBarController.Start();
        staminaBarController.Start();
    }

    void Update()
    {
        healthBarController.Update(player.Health.Value);
        oxygenBarController.Update(player.Oxygen.Value);
        staminaBarController.Update(player.Stamina.Value, player.Stamina.IsCooldown);
    }
}

}
