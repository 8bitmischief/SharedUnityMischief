using System;
using UnityEngine;

namespace SharedUnityMischief
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
	public class ShowIfEnumAttribute : PropertyAttribute
	{
		public string nameOfPropertyToCheck = "";
		public string[] allowedValues;

		public ShowIfEnumAttribute(string nameOfPropertyToCheck, params string[] allowedValues)
		{
			this.nameOfPropertyToCheck = nameOfPropertyToCheck;
			this.allowedValues = allowedValues;
		}
	}
}