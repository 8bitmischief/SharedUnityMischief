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
				effect.Play(false);
			}
			if (GUILayout.Button("Play Endlessly"))
			{
				effect.PlayEndlessly();
			}
			if (GUILayout.Button("Stop"))
			{
				effect.Stop();
			}
		}
	}
}