using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
 
[CustomPropertyDrawer(typeof(StateMachine))]
public class StateMachinePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        EditorGUI.BeginProperty(position, label, property);
        
        EditorGUILayout.Space();
        
        GUIStyle headerStyle = new GUIStyle("BoldLabel");
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.normal.textColor = GUI.skin.label.normal.textColor;
        EditorGUILayout.LabelField(property.FindPropertyRelative("ownerName").stringValue + "'s State Machine", headerStyle, GUILayout.ExpandWidth(true));
        SerializedProperty states = property.FindPropertyRelative("states");
        EditorGUI.indentLevel += 1;
        if (states.isExpanded) 
        {
            for (int i = 0; i < states.arraySize; i++) 
            {
                EditorGUILayout.PropertyField(states.GetArrayElementAtIndex(i));
            }
        }
        EditorGUI.indentLevel -= 1;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        EditorGUILayout.Space();

        EditorGUI.EndProperty();
    }
}

