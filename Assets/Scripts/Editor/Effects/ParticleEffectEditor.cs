using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Effects
{
	[CustomEditor(typeof(ParticleEffect), true)]
	public class ParticleEffectEditor : BaseEditor
	{
		protected override void DrawControls()
		{
			ParticleEffect effect = (ParticleEffect) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Particle Effect Controls", EditorStyles.boldLabel);
			if (GUILayout.Button("Play"))
			{
				effect.Play();
			}
			if (GUILayout.Button(effect.isPlayingEndlessly ? "Stop Playing Endlessly" : "Play Endlessly"))
			{
				if (effect.isPlayingEndlessly)
					effect.Stop();
				else
					effect.PlayEndlessly();
			}
			if (GUILayout.Button("Stop"))
			{
				effect.Stop();
			}
		}

		protected override void DrawEditModeControls()
		{
			DrawControls();
		}
	}
}