using System;
using UnityEngine;

namespace SharedUnityMischief
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
	public class ShowIfBoolAttribute : PropertyAttribute
	{
		public string nameOfPropertyToCheck = "";
		public bool valueToCheckFor = true;

		public ShowIfBoolAttribute(string nameOfPropertyToCheck, bool valueToCheckFor = true)
		{
			this.nameOfPropertyToCheck = nameOfPropertyToCheck;
			this.valueToCheckFor = valueToCheckFor;
		}
	}
}