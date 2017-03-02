using UnityEngine;

namespace GraphTool.Test
{
	public class TransformObserver : MonoBehaviour
	{
		public GraphHandler graph;

		[GraphDataKey("graph")]
		public int posX=-1, posY = -1, posZ = -1;
		public Transform target;


		private void Update()
		{
			if(graph != null && target != null)
			{
				graph.SetValue(posX, target.position.x);
				graph.SetValue(posY, target.position.y);
				graph.SetValue(posZ, target.position.z);
			}
		}
	}
}