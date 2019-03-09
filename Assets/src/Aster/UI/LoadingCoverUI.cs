using UnityEngine;
using UnityEngine.UI;
using Aster.World.Generation;

namespace Aster.UI {

public class LoadingCoverUI: MonoBehaviour
{
    public MapGenerator generator;
    public Text chunksLabel;
    public Image coverPlane;

    private bool decay = false;


    private void Update()
    {
        if (decay)
        {
            coverPlane.color = new Color(
                coverPlane.color.r,
                coverPlane.color.g,
                coverPlane.color.b,
                coverPlane.color.a - .01f
            );

            if (coverPlane.color.a < .01f)
            {
                decay = false;
                gameObject.SetActive(false);
            }
        }

        int left = generator.ProcessingChunksCount;
        int total = generator.TotalChunks;
        chunksLabel.text = "Constructing chunks " + (total - left) + "/" + total + "...";

        if (left <= 0)
        {
            decay = true;

            int n = coverPlane.transform.childCount;
            for (int i = 0; i < n; i++)
            {
                coverPlane.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}

}

