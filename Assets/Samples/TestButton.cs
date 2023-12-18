using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InspectorButtonAttribute : PropertyAttribute
{
    public string methodName;

    public InspectorButtonAttribute(string methodName)
    {
        this.methodName = methodName;
    }
}

[CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
public class InspectorButtonDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label);

        InspectorButtonAttribute attribute = (InspectorButtonAttribute)this.attribute;
        GameObject targetObject = property.serializedObject.targetObject as GameObject;

        if (GUI.Button(new Rect(position.x + position.width + 4, position.y, 100, EditorGUIUtility.singleLineHeight), "Execute"))
        {
            targetObject.SendMessage(attribute.methodName);
        }
    }
}