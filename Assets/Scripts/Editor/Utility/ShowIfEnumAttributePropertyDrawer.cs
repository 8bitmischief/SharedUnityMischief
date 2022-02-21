using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief
{
	[CustomPropertyDrawer(typeof(ShowIfEnumAttribute))]
	public class ShowIfEnumAttributePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			ShowIfEnumAttribute showIfEnumAttribute = (ShowIfEnumAttribute) attribute;
			if (ShouldShowProperty(showIfEnumAttribute, property))
			{
				bool wasEnabled = GUI.enabled;
				GUI.enabled = true;
				EditorGUI.PropertyField(position, property, label, true);
				GUI.enabled = wasEnabled;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			ShowIfEnumAttribute showIfEnumAttribute = (ShowIfEnumAttribute)attribute;
			if (ShouldShowProperty(showIfEnumAttribute, property))
				return EditorGUI.GetPropertyHeight(property, label);
			else
				return -EditorGUIUtility.standardVerticalSpacing;
		}

		private bool ShouldShowProperty(ShowIfEnumAttribute showIfEnumAttribute, SerializedProperty property)
		{
			// Construct a path to the field
			string pathToPropertyToCheck;
			int index = property.propertyPath.LastIndexOf(property.name);
			if ( index == -1 )
				throw new Exception($"Could not construct path to property \"{showIfEnumAttribute.nameOfPropertyToCheck}\" using path \"{property.propertyPath}\" for [ShowIfEnum]");
			pathToPropertyToCheck = property.propertyPath.Remove(index, property.name.Length).Insert(index, showIfEnumAttribute.nameOfPropertyToCheck);
			SerializedProperty propertyToCheck = property.serializedObject.FindProperty(pathToPropertyToCheck);

			if (propertyToCheck == null || propertyToCheck.enumNames.Length == 0 || propertyToCheck.enumValueIndex < 0 || propertyToCheck.enumValueIndex >= propertyToCheck.enumNames.Length)
				throw new Exception($"Could not find property \"{showIfEnumAttribute.nameOfPropertyToCheck}\" for [ShowIfEnum]");
			else
			{
				string propertyValue = propertyToCheck.enumNames[propertyToCheck.enumValueIndex];
				return showIfEnumAttribute.allowedValues.Contains(propertyValue);
			}
		}
	}
}