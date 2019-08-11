using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Aster.Objects;

namespace EditorExtensions {

[CustomEditor(typeof(Rope))]
public class RopeEditor: Editor
{
    private Rope item;

    private float SpawnDistance = 0.5f;
    private float PullForce = 1f;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20f);

        GUILayout.Label("Points Count: " + item.PointCount.ToString());
        GUILayout.Space(5f);

        GUILayout.Label("Spawn Distance: " + SpawnDistance.ToString());
        SpawnDistance = GUILayout.HorizontalSlider(SpawnDistance, 0f, 2f);


        if (GUILayout.Button("Spawn Segment"))
        {
            item.SpawnSegment(SpawnDistance);
        }


        GUILayout.Space(5f);
        GUILayout.Label("Pull force: " + PullForce.ToString());
        PullForce = GUILayout.HorizontalSlider(PullForce, 0f, 10f);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Pull Forward"))
        {
            item.PullHead(item.Head.transform.forward * PullForce);
        }
        if (GUILayout.Button("Pull Uprward"))
        {
            item.PullHead(item.Head.transform.up * PullForce);
        }
        if (GUILayout.Button("Pull Right"))
        {
            item.PullHead(item.Head.transform.right * PullForce);
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Attach Cargo"))
        {
            item.AttachCargo();
        }
    }

    private void OnEnable()
    {
        item = (Rope) target;
    }
}

}
