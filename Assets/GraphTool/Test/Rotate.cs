using UnityEngine;

namespace GraphTool.Test
{
	public class Rotate : MonoBehaviour
	{
		public float speed;
		public Vector3 axis;

		private void Update()
		{
			transform.Rotate(axis, speed * Time.deltaTime);
		}

	}
}