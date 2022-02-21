using System;
using UnityEngine;

namespace SharedUnityMischief
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
	public class ShowIfBoolAttribute : PropertyAttribute
	{
		public string nameOfPropertyToCheck = "";

		public ShowIfBoolAttribute(string nameOfPropertyToCheck)
		{
			this.nameOfPropertyToCheck = nameOfPropertyToCheck;
		}
	}
}