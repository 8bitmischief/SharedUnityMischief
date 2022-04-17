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

		public static float Round(float value, int decimals = 0)
		{
			float multiplier = Mathf.Pow(10f, decimals);
			return Mathf.Round(value * multiplier) / multiplier;
		}

		public static Vector3 Round(Vector3 vector, int decimals = 0)
		{
			float multiplier = Mathf.Pow(10f, decimals);
			return new Vector3(
				Mathf.Round(vector.x * multiplier) / multiplier,
				Mathf.Round(vector.y * multiplier) / multiplier,
				Mathf.Round(vector.z * multiplier) / multiplier);
		}

		public static Vector3 CalculatePlaneLineIntersection(Vector3 pointOnPlane, Vector3 planeNormal, Vector3 pointOnLine, Vector3 lineDirection) {
			return pointOnLine - lineDirection * Vector3.Dot(pointOnLine - pointOnPlane, planeNormal) / Vector3.Dot(lineDirection, planeNormal);
		}

		public static float CalculateChangeInSlope(Vector3 planeNormal, Vector3 vector, Vector3 down)
		{
			Vector3 intersectionPoint = CalculatePlaneLineIntersection(Vector3.zero, planeNormal, vector, down);
			Vector3 slopeVector = intersectionPoint - vector;
			return -Vector3.Dot(slopeVector, down);
		}
	}
}