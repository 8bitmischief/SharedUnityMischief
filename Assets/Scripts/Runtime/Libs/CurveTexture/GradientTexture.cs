using UnityEngine;

namespace SharedUnityMischief
{
	// This class is a modified version of the one from https://github.com/slipster216/CurveTexture
	// LICENSE included in directory, thank you for the code!
	[CreateAssetMenu(fileName = "New Gradient", menuName = "SharedUnityMischief/Gradient")]
	public class GradientTexture : ScriptableObject 
	{
		[SerializeField] private Gradient _gradient;

		public Texture2D GenerateTexture()
		{
			Texture2D texture = new Texture2D(128, 1, TextureFormat.ARGB32, false, true);
			Color[] pixels = new Color[128];
			for (int i = 0; i < pixels.Length; ++i)
			{
				float t = ((float) i) / ((float) (pixels.Length - 1));
				pixels[i] = _gradient.Evaluate(t);
			}
			texture.SetPixels(pixels);
			texture.Apply(false, false);
			return texture;
		}
	}
}