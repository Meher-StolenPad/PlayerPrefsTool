using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class CreatePrefWizard : ScriptableWizard
{
    [SerializeField] string Key = "";

    [SerializeField] PlayerPrefsType type;

    [SerializeField] object value;

    private int valuetempint;
    private float valuetempfloat;
    private string valuetempString;
    private Vector2 valuetempVector2;
    private Vector3 valuetempVector3;
    private Vector4 valuetempVecotr4;
    private Color valuetempColor;
    private bool valuetempBool;

    // [SerializeField] PlayerPrefsType type;
    private void OnEnable()
    {
        
    }

    private void OnInspectorUpdate()
    {
        createButtonName = "Add " + Key + " to player prefs";

        helpString = "Please set the color of the light!";

        if (String.IsNullOrEmpty(Key)){
            errorString = "Please set Key name";
            isValid = false;
        }
        else
        {
            errorString = "";
            isValid = true;
        }
        maxSize = new Vector2(300, 300);

    }

    protected override bool DrawWizardGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Label("Add New Player Pref", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Separator();

        DrawValueField();

        GUILayout.EndVertical();

        return base.DrawWizardGUI();
    }
    private void OnWizardCreate()
    {

        if (Resources.FindObjectsOfTypeAll(typeof(PlayerPrefsWindow)).Length >= 1)
        {
           ((PlayerPrefsWindow)Resources.FindObjectsOfTypeAll(typeof(PlayerPrefsWindow))[0]).AddPlayerPref();
        }
    }
    private void DrawValueField()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("Value", EditorStyles.wordWrappedLabel);

        switch (type)
        {
            case PlayerPrefsType.Int:
                valuetempint = EditorGUILayout.IntField(valuetempint, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                break;
            case PlayerPrefsType.Float:
                valuetempfloat = EditorGUILayout.FloatField(valuetempfloat, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                break;
            case PlayerPrefsType.String:
                valuetempString = EditorGUILayout.TextField(valuetempString, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                break;
            case PlayerPrefsType.Vector2:
                valuetempVector2 = EditorGUILayout.Vector2Field("", valuetempVector2, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                break;
            case PlayerPrefsType.Vector3:
                valuetempVector3 = EditorGUILayout.Vector3Field("", valuetempVector3, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                break;
            case PlayerPrefsType.Vector4:
                valuetempVecotr4 = EditorGUILayout.Vector4Field("", valuetempVecotr4, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                break;
            case PlayerPrefsType.Color:
                valuetempColor = EditorGUILayout.ColorField(valuetempColor, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                break;
            case PlayerPrefsType.Bool:
                valuetempBool = EditorGUILayout.ToggleLeft("", valuetempBool, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                break;
        }
        GUILayout.EndHorizontal();
    }
}

// post request : "Game name"