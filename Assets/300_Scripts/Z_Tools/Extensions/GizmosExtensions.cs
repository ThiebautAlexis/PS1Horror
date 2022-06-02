using UnityEngine;

namespace HorrorPS1.Tools
{
	public static class GizmosExtensions
    {
        #region Disc
        public static void DrawWireDisc(Vector3 _centerPosition, float _radius, int _subDivisions = 20)
		{
			Vector3[] _positions = new Vector3[_subDivisions];
			float _angleGap = 360 / _subDivisions;
			float _angle = -180; 
			for (int i = 0; i < _subDivisions; i++)
			{
				_positions[i] = _centerPosition + new Vector3(Mathf.Cos(Mathf.Deg2Rad* _angle), 0, Mathf.Sin(Mathf.Deg2Rad * _angle))*_radius ;
				_angle += _angleGap; 
			}
			for (int i = 0; i < _subDivisions-1; i++)
			{
				Gizmos.DrawLine(_positions[i], _positions[i + 1]); 
			}
			Gizmos.DrawLine(_positions[_subDivisions - 1], _positions[0]); 
		}

		public static void DrawDisc(Vector3 _centerPosition, float _radius, int _subDivisions = 20)
		{
			Vector3[] vertices = new Vector3[_subDivisions + 1];
			vertices[0] = Vector3.zero; 
			float _angleGap = 360 / _subDivisions;
			float _angle = 180;
			for (int i = 1; i < vertices.Length; i++)
			{
				vertices[i] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * _angle), 0, Mathf.Sin(Mathf.Deg2Rad * _angle)) * _radius;
				_angle -= _angleGap;
			}
			int[] triangles = new int[_subDivisions * 3];
			int _index = 1; 
			for (int i = 0; i < triangles.Length; i+=3)
			{
				triangles[i] = 0;
				triangles[i + 1] = _index;
				_index++;
				if (_index == vertices.Length)
					_index = 1;
				triangles[i + 2] = _index;
			}
			Vector3[] normals = new Vector3[vertices.Length];
			for (int i = 0; i < normals.Length; i++)
			{
				normals[i] = Vector3.up; 
			}
			Vector2[] uv = new Vector2[vertices.Length]; 
			Mesh _mesh = new Mesh();
			_mesh.vertices = vertices;
			_mesh.triangles = triangles; 
			_mesh.normals = normals;
			Gizmos.DrawMesh(_mesh, _centerPosition, Quaternion.identity);
		}
        #endregion

        #region Arrow
        public static void DrawArrow(Vector3 _origin, Vector3 _direction)
		{
			Vector3 _end = _origin + _direction; 
			Gizmos.DrawLine(_origin, _end);

			Vector3 _right = Quaternion.LookRotation(_direction) * Quaternion.Euler(0, 180 + 20, 0) * new Vector3(0, 0, 1);
			Vector3 _left = Quaternion.LookRotation(_direction) * Quaternion.Euler(0, 180 - 20, 0) * new Vector3(0, 0, 1);
			Gizmos.DrawRay(_end, _right * .25f);
			Gizmos.DrawRay(_end, _left * .25f);
		}

		public static void DrawArrowToward(Vector3 _origin, Vector3 _end)
		{
			Vector3 _direction = _end - _origin; 
			Gizmos.DrawLine(_origin, _end);

			Vector3 _right = Quaternion.LookRotation(_direction) * Quaternion.Euler(0, 180 + 20, 0) * new Vector3(0, 0, 1);
			Vector3 _left = Quaternion.LookRotation(_direction) * Quaternion.Euler(0, 180 - 20, 0) * new Vector3(0, 0, 1);
			Gizmos.DrawRay(_end, _right * .25f);
			Gizmos.DrawRay(_end, _left * .25f);
		}

		public static void DrawPointingArrow(Vector3 _point, Vector3 _orientation) => DrawArrowToward(_point - _orientation, _point);
        #endregion
    }
}
