using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Effects
{
	[CustomEditor(typeof(ParticleEffect), true)]
	public class ParticleEffectEditor : BaseEditor
	{
		public override bool RequiresConstantRepaint() => Application.isPlaying;

		protected override void DrawControls()
		{
			ParticleEffect effect = (ParticleEffect) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Particle Effect Controls", EditorStyles.boldLabel);
			if (GUILayout.Button("Play Once"))
			{
				effect.PlayOnce();
			}
			if (GUILayout.Button(effect.isLooping ? "Stop Looping" : "Play (Looping)"))
			{
				if (effect.isLooping)
					effect.StopLooping();
				else
					effect.PlayLooping();
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

		protected override void DrawState()
		{
			ParticleEffect effect = (ParticleEffect) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Particle Effect State", EditorStyles.boldLabel);
			EditorGUILayout.Toggle("IsPlaying", effect.isPlaying);
		}
	}
}