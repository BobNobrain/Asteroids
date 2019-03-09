using UnityEngine;
using UnityEngine.UI;
using Aster.Utils.Bars;

namespace Aster.UI.BarControllers {

[System.Serializable]
public class SimpleBarController
{
    public Slider uiBar;
    public Image uiBg;
    public Image uiFg;

    public Color dangerColor;
    protected Color dangerBgColor;
    protected Color regularBgColor;
    protected Color regularFgColor;

    [Range(0f, 1f)]
    public float dangerThreshold = .25f;

    public float BlinkSpeed = 3f;

    protected bool danger, blink;
    protected float dBlink;

    public virtual void Start()
    {
        regularFgColor = uiFg.color;
        regularBgColor = uiBg.color;

        dangerBgColor = new Color(dangerColor.r, dangerColor.g, dangerColor.b, regularBgColor.a);
    }

    public virtual void Update(float value)
    {
        uiBar.value = value;
        SwitchDanger(value);
        SwitchBlink(danger);
        if (blink)
        {
            Blink();
        }
    }

    protected virtual void SwitchDanger(float value)
    {
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
    }

    protected virtual void SwitchBlink(bool newBlinkValue)
    {
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
    }

    protected virtual void Blink()
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
