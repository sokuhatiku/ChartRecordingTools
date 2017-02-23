using System.Collections;
using UnityEngine;


namespace GraphTool.Test
{

	[RequireComponent(typeof(GraphHandler))]
	public class ValueGenerator : MonoBehaviour
	{
		[SerializeField]
		GraphHandler graph;
		[GraphDataKey("graph")]
		public int sin = -1, cos = -1, tan = -1;
		public float interval = 1f;



#if UNITY_EDITOR

		private void Reset()
		{
			graph = GetComponent<GraphHandler>();
		}

#endif

		private void OnEnable()
		{
			if ( graph == null)
			{
				enabled = false;
				return;
			}
			StartCoroutine(generateValue());
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}

		IEnumerator generateValue()
		{
			while (true)
			{
				graph.SetData(sin, Mathf.Sin(Time.time));
				graph.SetData(cos, Mathf.Cos(Time.time));
				graph.SetData(sin, Mathf.Tan(Time.time));
				yield return new WaitForSeconds(interval);
			}
		}
	}

}