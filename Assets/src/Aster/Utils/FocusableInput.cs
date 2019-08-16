using UnityEngine;

namespace Aster.Utils
{

public class FocusableInput
{
    public bool HasFocus { get; private set; }

    public FocusableInput() {}

    private static FocusableInput active = null;
    public static FocusableInput Active { get { return active; } }

    /// <summary>
    /// Creates a new FocusableInput and returns it. If there is no input that has focus, focuses it.
    /// </summary>
    /// <returns>Probably focused instance</returns>
    public static FocusableInput Create()
    {
        var result = new FocusableInput();
        Debug.Log("Create called");
        if (active == null)
        {
            // activate the first input
            result.HasFocus = true;
            active = result;
            Debug.Log("Activated!");
        }
        return result;
    }

    public void TransferFocus(FocusableInput to)
    {
        if (!HasFocus) return;

        HasFocus = false;
        to.HasFocus = true;
        active = to;
    }

    public bool GetButton(string buttonName)
    {
        if (HasFocus) return Input.GetButton(buttonName);
        return false;
    }
    public bool GetButtonDown(string buttonName)
    {
        if (HasFocus) return Input.GetButtonDown(buttonName);
        return false;
    }
    public bool GetButtonUp(string buttonName)
    {
        if (HasFocus) return Input.GetButtonUp(buttonName);
        return false;
    }

    public float GetAxis(string axis)
    {
        if (HasFocus) return Input.GetAxis(axis);
        return 0f;
    }
    public float GetAxisRaw(string axis)
    {
        if (HasFocus) return Input.GetAxisRaw(axis);
        return 0f;
    }
}

}
