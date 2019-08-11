using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Aster.UI
{

[System.Serializable]
public class DecayableHint
    {
    private UIManager ui;
    public Text label;
    private float originalAlpha;

    public void Init(UIManager m)
    {
        originalAlpha = label.color.a;
    }

    public void SetHint(string hint)
    {
        label.text = hint;
    }
    public void SetHint(string hint, float decayTime)
    {
        label.text = hint;

        ui.StartCoroutine(DecayHint(decayTime));
    }
    private IEnumerator DecayHint(float time)
    {
        float passed = 0;
        float opt = originalAlpha / time;
        while (passed < time)
        {
            passed += Time.unscaledDeltaTime;
            label.color = new Color(label.color.r, label.color.g, label.color.b, passed * opt);
            yield return new WaitForEndOfFrame();
        }

        label.color = new Color(label.color.r, label.color.g, label.color.b, originalAlpha);
        label.enabled = false;
    }
}

}