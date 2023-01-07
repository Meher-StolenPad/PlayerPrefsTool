using System;
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
    private string oldKey;

    private void OnInspectorUpdate()
    {
        createButtonName = "Add " + Key + " to player prefs";

        helpString = "Note : you cant' add a new key that already have a value !";

        if (String.IsNullOrEmpty(Key))
        {
            errorString = "Please set Key name";
            isValid = false;
        }
        else
        {
            if (Key != oldKey)
            {
                if (PlayerPrefs.HasKey(Key))
                {
                    errorString = "This Key has already a value";
                    isValid = false;
                }
                else
                {
                    errorString = "";
                    isValid = true;
                }
                oldKey = Key;
            }
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
            ((PlayerPrefsWindow)Resources.FindObjectsOfTypeAll(typeof(PlayerPrefsWindow))[0]).AddPlayerPref(Key, type, value);
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
                value = valuetempint;
                break;
            case PlayerPrefsType.Float:
                valuetempfloat = EditorGUILayout.FloatField(valuetempfloat, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                value = valuetempfloat;
                break;
            case PlayerPrefsType.String:
                valuetempString = EditorGUILayout.TextField(valuetempString, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                value = valuetempString;
                break;
            case PlayerPrefsType.Vector2:
                valuetempVector2 = EditorGUILayout.Vector2Field("", valuetempVector2, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                value = valuetempVector2;
                break;
            case PlayerPrefsType.Vector3:
                valuetempVector3 = EditorGUILayout.Vector3Field("", valuetempVector3, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                value = valuetempVector3;
                break;
            case PlayerPrefsType.Vector4:
                valuetempVecotr4 = EditorGUILayout.Vector4Field("", valuetempVecotr4, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                value = valuetempVecotr4;
                break;
            case PlayerPrefsType.Color:
                valuetempColor = EditorGUILayout.ColorField(valuetempColor, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                value = valuetempColor;
                break;
            case PlayerPrefsType.Bool:
                valuetempBool = EditorGUILayout.ToggleLeft("", valuetempBool, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                value = valuetempBool;
                break;
        }
        GUILayout.EndHorizontal();
    }
}