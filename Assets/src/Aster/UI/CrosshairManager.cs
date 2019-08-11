using UnityEngine.UI;

namespace Aster.UI
{
public enum CrosshairType { DEFAULT, INTERACTIVE }

[System.Serializable]
public class CrosshairManager: UISubManager
{
    public Image crosshair;
    public Image interactiveCrosshair;
    public DecayableHint topHint;
    public DecayableHint bottomHint;

    public override void Init(UIManager m)
    {
        base.Init(m);
        topHint.Init(m);
        bottomHint.Init(m);
    }

    public void SetType(CrosshairType t)
    {
        switch (t)
        {
            case CrosshairType.DEFAULT:
            interactiveCrosshair.enabled = false;
            break;

            case CrosshairType.INTERACTIVE:
            interactiveCrosshair.enabled = true;
            break;
        }
    }
}
}
