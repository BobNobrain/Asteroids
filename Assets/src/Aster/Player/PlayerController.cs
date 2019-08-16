using UnityEngine;
using Aster.Tools.Weapons;
using Aster.UI;
using Aster.Utils;

namespace Aster.Player {

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerController: MonoBehaviour
{
    #region References
    [HideInInspector] public Rigidbody body;
    [HideInInspector] public PlayerStats stats;

    // TODO: refactor
    public RopeGun CurrentWeapon;
    public GameObject BulletsRoot;

    public Transform raycaster;
    public UIManager playerUI;

    public PlayerActor actor;

    private FocusableInput input;
    #endregion

    #region Subcontrollers
    private ZeroGMover zeroGMover;
    private GravitationalMover gravitationalMover;
    private PlayerMover activeMover;

    private PlayerInteractor interactor;
    #endregion

    #region Settings
    public ZeroGMover.Settings zeroGMoveSettings;
    public GravitationalMover.Settings gravitationalMoveSettings;

    public PlayerInteractor.Settings interactionSettings;
    #endregion

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        stats = GetComponent<PlayerStats>();

        input = FocusableInput.Create();

        zeroGMover = new ZeroGMover(zeroGMoveSettings, this, input);
        gravitationalMover = new GravitationalMover(gravitationalMoveSettings, this, input);
        activeMover = zeroGMover;

        interactor = new PlayerInteractor(interactionSettings, this);

        actor = new PlayerActor(this);
        actor.Inventory.ContentChanged += playerUI.inventory.OnInventoryChanged;
        playerUI.inventory.OnInventoryChanged(actor.Inventory);

        zeroGMover.Awake();
        gravitationalMover.Awake();
        interactor.Awake();
    }

    void Update()
    {
        if (input.GetButton("Cancel"))
        {
            Debug.Log("Quit");
            Application.Quit();
            return;
        }

        if (input.GetButtonUp("Inventory"))
        {
            playerUI.inventory.SetVisibility(true, input);
        }

        activeMover.Update();
        interactor.Update();
    }

    private void FixedUpdate()
    {
        activeMover.FixedUpdate();
        interactor.FixedUpdate();
    }
}

}
