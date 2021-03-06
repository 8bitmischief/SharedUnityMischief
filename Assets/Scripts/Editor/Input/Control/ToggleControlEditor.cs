using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Input.Control
{
	[CustomEditor(typeof(ToggleControl), true)]
	public class ToggleControlEditor : BaseEditor
	{
		private float _lastToggleTime = -999f;

		public override bool RequiresConstantRepaint() => Application.isPlaying;

		private void OnEnable()
		{
			ToggleControl control = (ToggleControl) target;
			control.onToggle += OnToggle;
		}

		private void OnDisable()
		{
			ToggleControl control = (ToggleControl) target;
			control.onToggle += OnToggle;
		}

		protected override void DrawState()
		{
			ToggleControl control = (ToggleControl) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
			EditorGUILayout.Toggle("Is On", control.isOn);

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
			float toggleHighlight = 1f - Math.Map(Time.time - _lastToggleTime, 0.1f, 0.4f, 0f, 1f, true);
			EditorGUILayout.ColorField("Toggle", new Color(toggleHighlight, toggleHighlight, 0.0f, 1.0f));
		}

		private void OnToggle(bool isOn)
		{
			_lastToggleTime = Time.time;
		}
	}
}