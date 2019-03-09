using UnityEngine;
using UnityEngine.UI;
using Aster.Utils.Bars;

namespace Aster.UI.BarControllers {

[System.Serializable]
public class CooldownBarController: SimpleBarController
{
    public bool BlinkWhenDanger = false;

    public void Update(float value, bool cooldown)
    {
        uiBar.value = value;
        SwitchDanger(value);
        SwitchBlink(cooldown || (BlinkWhenDanger && danger));
        if (blink)
        {
            Blink();
        }
    }
}

}
