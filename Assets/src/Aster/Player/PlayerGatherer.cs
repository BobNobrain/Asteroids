using UnityEngine;
using UnityEngine.UI;
using Aster.Objects;

public class PlayerGatherer: MonoBehaviour
{
    public Text crosshairLabel;
    private int gatherableItemsMask;
    public Transform raycaster;
    public float maxGatherDistance;

    private void Awake()
    {
        gatherableItemsMask = LayerMask.NameToLayer("Gatherables");
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(raycaster.position, raycaster.forward, out hit, maxGatherDistance, 1 << gatherableItemsMask))
        {
            var g = hit.transform.GetComponent<GatherableObject>();
            if (g != null && g.gatherableItem != null)
            {
                crosshairLabel.text = g.gatherableItem.type.name;
                return;
            }
        }

        crosshairLabel.text = "";
    }
}
