using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief {
	[CustomPropertyDrawer(typeof(Curve), true)]
	public class CurvePropertyDrawer : PropertyDrawer {
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			EditorGUI.PropertyField(position, property.FindPropertyRelative("_animationCurve"), GUIContent.none);
			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}
	}
}