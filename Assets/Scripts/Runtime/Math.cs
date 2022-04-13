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

		public static Vector3 Round(Vector3 vector, int decimals = 0)
		{
			float multiplier = Mathf.Pow(10f, decimals);
			return new Vector3(
				Mathf.Round(vector.x * multiplier) / multiplier,
				Mathf.Round(vector.y * multiplier) / multiplier,
				Mathf.Round(vector.z * multiplier) / multiplier);
		}
	}
}