using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Input.Control {
	[CustomEditor(typeof(Vector2Control), true)]
	public class Vector2ControlEditor : BaseEditor {
		public override bool RequiresConstantRepaint () => Application.isPlaying;

		protected override void DrawState () {
			Vector2Control control = (Vector2Control) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
			EditorGUILayout.Vector2Field("Vector", control.vector);
		}
	}
}