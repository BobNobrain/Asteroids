using UnityEngine;
using Aster.Objects;
using Aster.UI;

namespace Aster.Player
{

public class PlayerInteractor: PlayerControllerPart
{
    private Settings settings;
    private InteractableObject target;
    private bool lastFrameWeHadTarget = false;

    public PlayerInteractor(Settings settings, PlayerController controller): base(controller)
    {
        this.settings = settings;
    }

    private RaycastHit hit;
    private int interactableItemsMask;

    public override void Awake()
    {
        interactableItemsMask = LayerMask.NameToLayer("Interactables");

        ClearUI();
    }

    public override void Update()
    {
        // if (Input.GetButton("Fire1") && CurrentWeapon != null)
        // {
        //     CurrentWeapon.PrimaryTrigger();
        // }

        InteractableObject newTarget = null;
        if (Physics.Raycast(
            player.raycaster.position,
            player.raycaster.forward,
            out hit,
            settings.maxGatherDistance,
            1 << interactableItemsMask
        ))
        {
            newTarget = hit.transform.GetComponent<InteractableObject>();
        }

        if (newTarget != target)
        {
            UpdateUI(newTarget);
            target = newTarget;
        }
        else if (lastFrameWeHadTarget && newTarget == null)
        {
            UpdateUI(null);
        }

        lastFrameWeHadTarget = false;
        if (target != null)
        {
            lastFrameWeHadTarget = true;
            if (Input.GetButton("Fire1"))
            {
                target.Interact(player.actor);
            }
        }
    }

    private void UpdateUI(InteractableObject newTarget)
    {
        if (newTarget != null)
        {
            player.playerUI.crosshair.topHint.SetHint(newTarget.Name);
            if (target == null)
            {
                player.playerUI.crosshair.SetType(CrosshairType.INTERACTIVE);
                player.playerUI.crosshair.bottomHint.SetHint("Press (LMB) to interact");
            }
        }
        else
        {
            ClearUI();
        }
    }
    private void ClearUI()
    {
        player.playerUI.crosshair.SetType(CrosshairType.DEFAULT);
        player.playerUI.crosshair.topHint.SetHint("");
        player.playerUI.crosshair.bottomHint.SetHint("");
    }

    [System.Serializable]
    public class Settings
    {
        public float maxGatherDistance = 2f;
    }
}

}
