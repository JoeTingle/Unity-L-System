using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Editor Class For The L System To Allow Developers To Modify and Test Within Unity Inspector
/// </summary>
[CustomEditor(typeof(LSystem))]
public class LSystemEditor : Editor
{
    /// <summary>
    /// Used To Display UI (Custom Buttons/Dropdowns etc.) On Unity Inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //Creating the target script for this to be attached to
        LSystem lSystemScript = (LSystem)target;
        if (GUILayout.Button("Generate Tree")) // Button which will generate a tree in editor when pressed
        {
            lSystemScript.EditorGenerate();
        }
        if (GUILayout.Button("Clear")) // Button which will clear all currently generated tree components
        {
            for (int i = 0; i < 20; i++)
            {
                lSystemScript.Clear();
                //Also parses to reset
                lSystemScript.ParseTreeFile();
            }
        }
        if (GUILayout.Button("Parse Tree Preset")) //Button to manually parse the file
        {
          lSystemScript.ParseTreeFile();
        }
    }
}