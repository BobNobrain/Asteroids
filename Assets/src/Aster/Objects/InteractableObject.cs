using UnityEngine;
using Aster.Actors;

namespace Aster.Objects
{

public class InteractableObject: MonoBehaviour
{
    public virtual void Interact(Actor a)
    {
        Debug.Log("Interact!");
    }

    public virtual bool CanInteract(Actor a) { return true; }

    public virtual string Name { get { return "[interactable]"; } }
}

}
