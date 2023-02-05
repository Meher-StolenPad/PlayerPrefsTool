#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrivacyPopup : ScriptableWizard
{
    [HideInInspector]
    public string PrivacyLink;
    protected override bool DrawWizardGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Privacy Link", EditorStyles.wordWrappedLabel);
        EditorGUILayout.TextField(PrivacyLink);
        GUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        return base.DrawWizardGUI();
    }

    private void OnWizardCreate()
    {
        Close();
    }
    void OnWizardUpdate()
    {
        helpString = "Copy To Clip board";
    }
}
#endif