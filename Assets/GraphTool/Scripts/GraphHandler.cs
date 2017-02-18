using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GraphTool
{

	public class GraphHandler : MonoBehaviour
	{

		public GraphPlotter plotter;
		[SerializeField] Rect Scope = new Rect(0, 0, 100f, 200f);


		public void RegisterPlotter(GraphPlotter plotter)
		{
			this.plotter = plotter;
		}

		public GraphPlotter GetPlotter()
		{
			return plotter;
		}

		private void FixedUpdate()
		{
			if (plotter != null)
			{
				Scope.x = Time.time;
				plotter.UpdateScope(Scope);
			}
		}

	}

}