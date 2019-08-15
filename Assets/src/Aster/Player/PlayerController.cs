using UnityEngine;
using Aster.Tools.Weapons;
using Aster.UI;

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
    #endregion

    #region Subcontrollers
    private ZeroGMover zeroGMover;
    private GravitationalMover gravitationalMover;
    private PlayerControllerPart activeMover;

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

        zeroGMover = new ZeroGMover(zeroGMoveSettings, this);
        gravitationalMover = new GravitationalMover(gravitationalMoveSettings, this);
        activeMover = zeroGMover;

        interactor = new PlayerInteractor(interactionSettings, this);

        actor = new PlayerActor(this);

        zeroGMover.Awake();
        gravitationalMover.Awake();
        interactor.Awake();
    }

    void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            Debug.Log("Quit");
            Application.Quit();
            return;
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
