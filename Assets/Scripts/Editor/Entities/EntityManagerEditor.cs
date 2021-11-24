using UnityEngine;
using UnityEditor;

namespace SharedUnityMischief.Entities {
	[CustomEditor(typeof(EntityManager), true)]
	public class EntityManagerEditor : BaseEditor {
		public override bool RequiresConstantRepaint () => Application.isPlaying;

		protected override void DrawState () {
			EntityManager manager = (EntityManager) target;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Game State", EditorStyles.boldLabel);
			EditorGUILayout.IntField("Entities", manager.numEntities);
			EditorGUILayout.IntField("Entities To Spawn", manager.numEntitiesToSpawn);
			EditorGUILayout.IntField("Entities To Despawn", manager.numEntitiesToDespawn);
		}
	}
}