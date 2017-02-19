using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphTool.Test
{

	[RequireComponent(typeof(GraphHandler))]
	public class RandomValueGenerator : MonoBehaviour
	{
		//public string plotterName;
		public int key;
		public float interval = 1f;

		[Space]
		public float Max = 100;
		public float Min = -100;
		[Range(1, 10)]
		public int Richness = 1;

		[Space]
		public bool Continuity = false;
		public float Ct_Max = 100f;
		public float Ct_Min = -100f;
		

		GraphHandler graph;

#if UNITY_EDITOR
		private void OnValidate()
		{
			if(Max < Min)
				Min = Max;
			if (Ct_Max < Ct_Min)
				Ct_Min = Ct_Max;
		}
#endif

		private void OnEnable()
		{
			graph = GetComponent<GraphHandler>();
			StartCoroutine(generateRandomValue());
		}

		private void OnDisable()
		{
			StopAllCoroutines();
		}

		float value;
		IEnumerator generateRandomValue()
		{
			while (true)
			{
				var newValue = 0f;
				if (Max - Min > 0)
				{
					for (int i = 0; i < Richness; i++)
						newValue += Continuity ? Random.Range(Ct_Min, Ct_Max): Random.Range(Min, Max);
					newValue /= Richness;
				}
				else newValue = (Max + Min) / 2;
				if (Continuity) value = Mathf.Clamp(value + newValue, Min, Max);
				else value = newValue;
				graph.AddValue(key, value);
				yield return new WaitForSeconds(interval);
			}
		}
	}

}