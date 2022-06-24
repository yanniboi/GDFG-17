using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShadowCaster2DTileMap))]
public class ShadowCastersGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector();
        ShadowCaster2DTileMap generator = (ShadowCaster2DTileMap)this.target;
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();


        if (GUILayout.Button("Generate"))
        {

            generator.Generate();

        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Destroy All Children"))
        {

            generator.DestroyAllChildren();

        }
    }

}
