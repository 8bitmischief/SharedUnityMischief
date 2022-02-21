using UnityEngine;

namespace SharedUnityMischief
{
	public class DebugTarget : MonoBehaviour
	{
		[SerializeField] private Color _color = Color.yellow;
		[SerializeField] private float _size = 1f;
		[SerializeField] private bool _outline = false;
		[SerializeField] private bool _fill = true;
		[SerializeField] private DebugTargetMesh _mesh = DebugTargetMesh.Cube;
		[SerializeField, ShowIfEnum("_mesh", "Custom")] private Mesh _customMesh;

		private void OnDrawGizmos()
		{
			Matrix4x4 matrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;
			if (_fill)
			{
				Gizmos.color = new Color(_color.r, _color.g, _color.b, _color.a * 0.35f);
				switch (_mesh)
				{
					case DebugTargetMesh.Cube:
						Gizmos.DrawCube(Vector3.zero, new Vector3(_size, _size, _size));
						break;
					case DebugTargetMesh.Sphere:
						Gizmos.DrawSphere(Vector3.zero, 0.5f * _size);
						break;
					case DebugTargetMesh.Custom:
						if (_customMesh != null)
							Gizmos.DrawMesh(_customMesh, Vector3.zero, Quaternion.identity, new Vector3(_size, _size, _size));
						break;
				}
			}
			if (_outline)
			{
				Gizmos.color = _color;
				switch (_mesh)
				{
					case DebugTargetMesh.Cube:
						Gizmos.DrawWireCube(Vector3.zero, new Vector3(_size, _size, _size));
						break;
					case DebugTargetMesh.Sphere:
						Gizmos.DrawWireSphere(Vector3.zero, 0.5f * _size);
						break;
					case DebugTargetMesh.Custom:
						if (_customMesh != null)
							Gizmos.DrawWireMesh(_customMesh, Vector3.zero, Quaternion.identity, new Vector3(_size, _size, _size));
						break;
				}
			}
			Gizmos.matrix = matrix;
		}

		private enum DebugTargetMesh
		{
			None = 0,
			Cube = 1,
			Sphere = 2,
			Custom = 3
		}
	}
}