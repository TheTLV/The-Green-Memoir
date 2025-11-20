using UnityEngine;
using UnityEditor;
using TheGreenMemoir.Unity.Attributes;

namespace TheGreenMemoir.Unity.Editor
{
#if UNITY_EDITOR
    /// <summary>
    /// Custom property drawer để hiển thị Enum Flags trong Inspector
    /// </summary>
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        }
    }
#endif
}

