using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aster.Player
{

public class PlayerControllerPart
{
    protected PlayerController player;

    public PlayerControllerPart(PlayerController controller)
    {
        player = controller;
    }

    public virtual void Awake() {}
    public virtual void Update() {}
    public virtual void FixedUpdate() {}
}

}
