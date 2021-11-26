using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Input.Control
{
	[CustomEditor(typeof(ButtonControl), true)]
	public class ButtonControlEditor : BaseEditor
	{
		private float _lastPressTime = -999f;
		private float _lastReleaseTime = -999f;

		public override bool RequiresConstantRepaint() => Application.isPlaying;

		private void OnEnable()
		{
			ButtonControl control = (ButtonControl) target;
			control.onPress += OnPress;
			control.onRelease += OnRelease;
		}

		private void OnDisable()
		{
			ButtonControl control = (ButtonControl) target;
			control.onPress += OnPress;
			control.onRelease += OnRelease;
		}

		protected override void DrawState()
		{
			ButtonControl control = (ButtonControl) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
			EditorGUILayout.Toggle("Is Held", control.isHeld);
			EditorGUILayout.FloatField("Amount Held Down", control.amountHeldDown);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
			float pressHighlight = 1f - Math.Map(Time.time - _lastPressTime, 0.1f, 0.4f, 0f, 1f, true);
			EditorGUILayout.ColorField("Press", new Color(pressHighlight, pressHighlight, 0.0f, 1.0f));
			float releaseHighlight = 1f - Math.Map(Time.time - _lastReleaseTime, 0.1f, 0.4f, 0f, 1f, true);
			EditorGUILayout.ColorField("Release", new Color(releaseHighlight, releaseHighlight, 0.0f, 1.0f));
		}

		private void OnPress()
		{
			_lastPressTime = Time.time;
		}

		private void OnRelease()
		{
			_lastReleaseTime = Time.time;
		}
	}
}