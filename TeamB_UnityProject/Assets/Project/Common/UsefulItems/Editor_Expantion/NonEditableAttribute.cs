// インスペクターから値を変更させず、参照だけできるようにする Editor拡張
// 使い方は、attributeに NonEditableを追加するだけ。[NonEditable]と書く。
/*
[SerializeField, NonEditable] int i = 0;
*/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class NonEditableAttribute : PropertyAttribute{}

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof(NonEditableAttribute) )]
public sealed class NonEditableAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight ( SerializedProperty property, GUIContent label )
    {
        return EditorGUI.GetPropertyHeight( property, label, true );
    }

    public override void OnGUI ( Rect position, SerializedProperty property, GUIContent label )
    {
    	GUI.enabled = false;
    	EditorGUI.PropertyField( position, property, label, true );
    	GUI.enabled = true;
    }
}
#endif