using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Pool {
	[CustomEditor(typeof(PrefabPool), true)]
	public class PrefabPoolEditor : BaseEditor {
		public override bool RequiresConstantRepaint () => Application.isPlaying;

		protected override void DrawState () {
			PrefabPool pool = (PrefabPool) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Prefab Pool State", EditorStyles.boldLabel);
			EditorGUILayout.IntField("Available Instances", pool.numAvailableInstances);
			EditorGUILayout.IntField("Instances", pool.numInstances);
			base.DrawState();
		}
	}
}