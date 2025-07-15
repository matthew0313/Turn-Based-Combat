using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct SaveID
{
    public string value;
    public void SetNew() => value = Guid.NewGuid().ToString();
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SaveID))]
public class SaveIDEditor : PropertyDrawer
{
    const float buttonWidth = 120;
    const float padding = 2;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        GUI.enabled = false;
        Rect valueRect = position;
        valueRect.width -= buttonWidth + padding;
        SerializedProperty idProperty = property.FindPropertyRelative("value");
        EditorGUI.PropertyField(valueRect, idProperty, GUIContent.none);

        GUI.enabled = true;

        Rect buttonRect = position;
        buttonRect.x += position.width - buttonWidth;
        buttonRect.width = buttonWidth;
        if (GUI.Button(buttonRect, "Reset"))
        {
            property.FindPropertyRelative("value").stringValue = Guid.NewGuid().ToString();
        }

        EditorGUI.EndProperty();
    }
}
#endif