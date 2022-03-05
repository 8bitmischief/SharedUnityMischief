using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.UI
{
	[CustomEditor(typeof(TableLayoutGroup))]
	[CanEditMultipleObjects]
	public class TableLayoutGroupEditor : Editor
	{
		private SerializedProperty _padding;
		private SerializedProperty _childAlignment;
		private SerializedProperty _cellAlignment;
		private SerializedProperty _spacingBetweenColumns;
		private SerializedProperty _spacingBetweenRows;
		private SerializedProperty _forceExpandColumns;
		private SerializedProperty _forceExpandRows;
		private SerializedProperty _forceExpandCellWidth;
		private SerializedProperty _forceExpandCellHeight;
		private SerializedProperty _reverseColumns;
		private SerializedProperty _reverseRows;

		protected virtual void OnEnable()
		{
			_padding = serializedObject.FindProperty("m_Padding");
			_childAlignment = serializedObject.FindProperty("m_ChildAlignment");
			_cellAlignment = serializedObject.FindProperty("_cellAlignment");
			_spacingBetweenColumns = serializedObject.FindProperty("_spacingBetweenColumns");
			_spacingBetweenRows = serializedObject.FindProperty("_spacingBetweenRows");
			_forceExpandColumns = serializedObject.FindProperty("_forceExpandColumns");
			_forceExpandRows = serializedObject.FindProperty("_forceExpandRows");
			_forceExpandCellWidth = serializedObject.FindProperty("_forceExpandCellWidth");
			_forceExpandCellHeight = serializedObject.FindProperty("_forceExpandCellHeight");
			_reverseColumns = serializedObject.FindProperty("_reverseColumns");
			_reverseRows = serializedObject.FindProperty("_reverseRows");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(_padding, true);
			EditorGUILayout.PropertyField(_childAlignment, true);
			EditorGUILayout.PropertyField(_cellAlignment, true);
			EditorGUILayout.PropertyField(_spacingBetweenColumns, true);
			EditorGUILayout.PropertyField(_spacingBetweenRows, true);

			Rect rect = EditorGUILayout.GetControlRect();
			rect = EditorGUI.PrefixLabel(rect, -1, EditorGUIUtility.TrTextContent("Table Force Expand"));
			rect.width = Mathf.Max(50, (rect.width - 4) / 3);
			EditorGUIUtility.labelWidth = 50;
			ToggleLeft(rect, _forceExpandColumns, EditorGUIUtility.TrTextContent("Columns"));
			rect.x += rect.width + 2;
			ToggleLeft(rect, _forceExpandRows, EditorGUIUtility.TrTextContent("Rows"));
			EditorGUIUtility.labelWidth = 0;

			rect = EditorGUILayout.GetControlRect();
			rect = EditorGUI.PrefixLabel(rect, -1, EditorGUIUtility.TrTextContent("Cell Force Expand"));
			rect.width = Mathf.Max(50, (rect.width - 4) / 3);
			EditorGUIUtility.labelWidth = 50;
			ToggleLeft(rect, _forceExpandCellWidth, EditorGUIUtility.TrTextContent("Width"));
			rect.x += rect.width + 2;
			ToggleLeft(rect, _forceExpandCellHeight, EditorGUIUtility.TrTextContent("Height"));
			EditorGUIUtility.labelWidth = 0;

			rect = EditorGUILayout.GetControlRect();
			rect = EditorGUI.PrefixLabel(rect, -1, EditorGUIUtility.TrTextContent("Reverse"));
			rect.width = Mathf.Max(50, (rect.width - 4) / 3);
			EditorGUIUtility.labelWidth = 50;
			ToggleLeft(rect, _reverseColumns, EditorGUIUtility.TrTextContent("Columns"));
			rect.x += rect.width + 2;
			ToggleLeft(rect, _reverseRows, EditorGUIUtility.TrTextContent("Rows"));
			EditorGUIUtility.labelWidth = 0;

			serializedObject.ApplyModifiedProperties();
		}

		private void ToggleLeft(Rect position, SerializedProperty property, GUIContent label)
		{
			bool toggle = property.boolValue;
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.BeginChangeCheck();
			int oldIndent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			toggle = EditorGUI.ToggleLeft(position, label, toggle);
			EditorGUI.indentLevel = oldIndent;
			if (EditorGUI.EndChangeCheck())
			{
				property.boolValue = property.hasMultipleDifferentValues ? true : !property.boolValue;
			}
			EditorGUI.EndProperty();
		}
	}
}