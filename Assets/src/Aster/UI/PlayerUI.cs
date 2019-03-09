using System.Collections;
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

    public Image VignetteEffect;
    public float VignetteDecaySpeed;
    private bool coroutineActive = false;

    public Color HurtColor;

    void Start()
    {
        healthBarController.Start();
        oxygenBarController.Start();
        staminaBarController.Start();

        player.Health.changedListener = OnPlayerHurt;
    }

    void Update()
    {
        healthBarController.Update(player.Health.Value);
        oxygenBarController.Update(player.Oxygen.Value);
        staminaBarController.Update(player.Stamina.Value, player.Stamina.IsCooldown);
    }

    private void OnPlayerHurt(float amount)
    {
        Debug.Log("Amount " + amount);
        if (amount < 0) VignetteEffect.color = HurtColor;
        else return; // TODO: heal effect, when healing will be implemented
        if (!coroutineActive)
        {
            coroutineActive = true;
            StartCoroutine(VignetteDecay());
        }
    }
    private IEnumerator VignetteDecay()
    {
        while (!Mathf.Approximately(VignetteEffect.color.a, 0f))
        {
            VignetteEffect.color = new Color(
                VignetteEffect.color.r,
                VignetteEffect.color.g,
                VignetteEffect.color.b,
                Mathf.Lerp(VignetteEffect.color.a, 0, Time.deltaTime * VignetteDecaySpeed)
            );
            yield return null;
        }
        coroutineActive = false;
    }
}

}
