using UnityEngine;
using UnityEditor;
using Aster.World;

namespace EditorExtensions {

[CustomEditor(typeof(Asteroid))]
public class AsteroidEditor: Editor {

    private Asteroid item;
    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed && item.LiveReload)
            {
                item.Generate(0, item.resolution, true);
            }
        }

        if (GUILayout.Button("Regenerate"))
        {
            item.Generate(0, item.resolution, true);
        }
    }

    private void OnEnable()
    {
        item = (Asteroid) target;
    }
}

}
