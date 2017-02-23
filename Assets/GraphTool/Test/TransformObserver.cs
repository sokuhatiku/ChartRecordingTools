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
				graph.SetData(posX, target.position.x);
				graph.SetData(posY, target.position.y);
				graph.SetData(posZ, target.position.z);
			}
		}
	}
}