using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Input.Control
{
	[CustomEditor(typeof(FloatControl), true)]
	public class FloatControlEditor : BaseEditor
	{
		public override bool RequiresConstantRepaint() => Application.isPlaying;

		protected override void DrawState()
		{
			FloatControl control = (FloatControl) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
			EditorGUILayout.FloatField("Value", control.value);
		}
	}
}