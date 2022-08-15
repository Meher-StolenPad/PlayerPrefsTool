using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ImportPrefsWizard : ScriptableWizard
{
    // Company and product name for importing PlayerPrefs from other projects
    [SerializeField] string importCompanyName = "";
    [SerializeField] string importProductName = "";

    private void OnEnable()
    {
        importCompanyName = PlayerSettings.companyName;
        importProductName = PlayerSettings.productName;
    }

    private void OnInspectorUpdate()
    {
        //if (Resources.FindObjectsOfTypeAll(typeof(PlayerPrefsEditor)).Length == 0)
        //{
        //    // Close();
        //}
    }

    protected override bool DrawWizardGUI()
    {
        GUILayout.Label("Import PlayerPrefs from another project, also useful if you change product or company name", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Separator();
        return base.DrawWizardGUI();
    }

    private void OnWizardCreate()
    {
        if (Resources.FindObjectsOfTypeAll(typeof(PlayerPrefsWindow)).Length >= 1)
        {
            ((PlayerPrefsWindow)Resources.FindObjectsOfTypeAll(typeof(PlayerPrefsWindow))[0]).Import(importCompanyName, importProductName);
        }
    }
}
