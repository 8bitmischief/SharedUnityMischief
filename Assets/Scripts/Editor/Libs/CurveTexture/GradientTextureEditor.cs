using System.IO;
using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief
{
	[CustomEditor(typeof(GradientTexture))]
	public class GradientTextureEditor : BaseEditor 
	{
		protected override void DrawControls()
		{
			GradientTexture gradientTexture = target as GradientTexture;
			if (GUILayout.Button("Generate Texture"))
			{
				string path = AssetDatabase.GetAssetPath(gradientTexture);
				path = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".png");
				bool didAssetAlreadyExist = !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path));
				// Create the texture and save it to a PNG
				Texture2D texture = gradientTexture.GenerateTexture();
				byte[] bytes = texture.EncodeToPNG();
				DestroyImmediate(texture);
				File.WriteAllBytes(path, bytes);
				// Import the new image asset
				AssetDatabase.ImportAsset(path);
				if (!didAssetAlreadyExist)
				{
					TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
					textureImporter.sRGBTexture = true;
					textureImporter.alphaIsTransparency = true;
					textureImporter.wrapMode = TextureWrapMode.Clamp;
					textureImporter.filterMode = FilterMode.Point;
					textureImporter.mipmapEnabled = false;
					textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
					textureImporter.SaveAndReimport();
				}
			}
		}

		protected override void DrawEditModeControls() => DrawControls();
	}
}