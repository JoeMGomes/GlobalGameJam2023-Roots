using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(State))]
public class StatePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Color defaultBackgroundColor = GUI.backgroundColor;
        GUIStyle activeStyle = new GUIStyle("Button");
        activeStyle.normal.background = Texture2D.grayTexture;
        GUIStyle inactiveStyle = new GUIStyle("Button");
        inactiveStyle.normal.background = Texture2D.grayTexture;
        
        SerializedProperty stateName = property.FindPropertyRelative("name");
        
        SerializedProperty stateIsActiveProperty = property.FindPropertyRelative("isActive");

        if (stateIsActiveProperty.boolValue)
        {
            GUI.backgroundColor = Color.green;
            EditorGUILayout.LabelField(stateName.stringValue, activeStyle);
        }
        else
        {
            GUI.backgroundColor = Color.red;
            EditorGUILayout.LabelField(stateName.stringValue, inactiveStyle);
        }
        GUI.backgroundColor = defaultBackgroundColor;
        
        EditorGUI.EndProperty();
    }
}
