using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Input.Control {
	[CustomEditor(typeof(LookControl), true)]
	public class LookControlEditor : BaseEditor {
		public override bool RequiresConstantRepaint () => Application.isPlaying;

		protected override void DrawState () {
			LookControl control = (LookControl) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
			EditorGUILayout.Vector2Field("Vector", control.vector);
			EditorGUILayout.Vector2Field("Recent Average Vector", control.recentAverageVector);
			EditorGUILayout.Toggle("Is Mouse Look Enabled", control.isMouseLookEnabled);
			EditorGUILayout.Toggle("Is Using Mouse Look", control.isUsingMouseLook);
		}
	}
}