using UnityEngine;

namespace SharedUnityMischief
{
	public static class Math
	{
		public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax, bool clamp = false)
		{
			float mappedValue = ((value - fromMin) / (fromMax - fromMin)) * (toMax - toMin) + toMin;
			if (clamp)
			{
				return Mathf.Clamp(mappedValue, Mathf.Min(toMin, toMax), Mathf.Max(toMin, toMax));
			}
			else
			{
				return mappedValue;
			}
		}
	}
}