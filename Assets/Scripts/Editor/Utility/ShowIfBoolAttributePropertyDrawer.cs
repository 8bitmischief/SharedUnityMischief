using System;
using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief
{
	[CustomPropertyDrawer(typeof(ShowIfBoolAttribute))]
	public class ShowIfBoolAttributePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			ShowIfBoolAttribute showIfBoolAttribute = (ShowIfBoolAttribute) attribute;
			if (ShouldShowProperty(showIfBoolAttribute, property))
			{
				bool wasEnabled = GUI.enabled;
				GUI.enabled = true;
				EditorGUI.PropertyField(position, property, label, true);
				GUI.enabled = wasEnabled;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			ShowIfBoolAttribute showIfBoolAttribute = (ShowIfBoolAttribute)attribute;
			if (ShouldShowProperty(showIfBoolAttribute, property))
				return EditorGUI.GetPropertyHeight(property, label);
			else
				return -EditorGUIUtility.standardVerticalSpacing;
		}

		private bool ShouldShowProperty(ShowIfBoolAttribute showIfBoolAttribute, SerializedProperty property)
		{
			string pathToPropertyToCheck;
			int index = property.propertyPath.LastIndexOf(property.name);
			if ( index == -1 )
				throw new Exception($"Could not construct path to property \"{showIfBoolAttribute.nameOfPropertyToCheck}\" using path \"{property.propertyPath}\" for [ShowIfBool]");
			pathToPropertyToCheck = property.propertyPath.Remove(index, property.name.Length).Insert(index, showIfBoolAttribute.nameOfPropertyToCheck);
			SerializedProperty propertyToCheck = property.serializedObject.FindProperty(pathToPropertyToCheck);
			if (propertyToCheck == null)
				throw new Exception($"Could not find property \"{showIfBoolAttribute.nameOfPropertyToCheck}\" for [ShowIfBool]");
			else
				return propertyToCheck.boolValue == showIfBoolAttribute.valueToCheckFor;
		}
	}
}