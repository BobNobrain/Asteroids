using UnityEngine;
using UnityEngine.UI;
using Aster.Player;

namespace Aster.UI {

public class PlayerUI: MonoBehaviour
{
    public PlayerStats player;
    public BarController healthBarController;
    public BarController oxygenBarController;
    public BarController staminaBarController;

    void Start()
    {
        healthBarController.Start();
        oxygenBarController.Start();
        staminaBarController.Start();

        healthBarController.blinkMode = BarController.BlinkMode.DANGER;
        oxygenBarController.blinkMode = BarController.BlinkMode.DANGER;
        staminaBarController.blinkMode = BarController.BlinkMode.COOLDOWN;
    }

    void Update()
    {
        healthBarController.Update(player.Health.Value, false);
        oxygenBarController.Update(player.Oxygen.Value, false);
        staminaBarController.Update(player.Stamina.Value, player.Stamina.IsCooldown);
    }

    [System.Serializable]
    public class BarController
    {
        public enum BlinkMode { OFF, COOLDOWN, DANGER };

        public Slider uiBar;
        public Image uiBg;
        public Image uiFg;

        public Color dangerColor;
        private Color dangerBgColor;
        private Color regularBgColor;
        private Color regularFgColor;

        [Range(0f, 1f)]
        public float dangerThreshold = .25f;

        public float BlinkSpeed = 3f;

        private bool danger, blink;
        private float dBlink;

        public BlinkMode blinkMode = BlinkMode.OFF;

        public void Start()
        {
            regularFgColor = uiFg.color;
            regularBgColor = uiBg.color;

            dangerBgColor = new Color(dangerColor.r, dangerColor.g, dangerColor.b, regularBgColor.a);
        }

        public void Update(float value, bool cooldown)
        {
            uiBar.value = value;
            if (value < dangerThreshold && !danger)
            {
                danger = true;
                uiFg.color = dangerColor;
                uiBg.color = dangerBgColor;
            }
            else if (value >= dangerThreshold && danger)
            {
                danger = false;
                uiBg.color = regularBgColor;
                uiFg.color = regularFgColor;
            }

            var newBlinkValue = false;
            switch (blinkMode)
            {
                case BlinkMode.DANGER:
                    newBlinkValue = danger;
                    break;

                case BlinkMode.COOLDOWN:
                    newBlinkValue = cooldown;
                    break;
            }

            if (!blink && newBlinkValue)
            {
                // start blinking
                dBlink = -BlinkSpeed;
            }
            else if (blink && !newBlinkValue)
            {
                uiFg.color = danger ? dangerColor : regularFgColor;
            }
            blink = newBlinkValue;

            if (blink)
            {
                float a = uiFg.color.a + dBlink * Time.deltaTime;
                if (a < 0f)
                {
                    a = 0f;
                    dBlink = BlinkSpeed;
                }
                else if (a > 1f)
                {
                    a = 1f;
                    dBlink = -BlinkSpeed;
                }
                uiFg.color = new Color(uiFg.color.r, uiFg.color.g, uiFg.color.b, a);
            }
        }
    }
}

}
