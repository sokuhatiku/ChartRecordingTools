/**
Graph Tool

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System.Runtime.InteropServices;
using UnityEngine;

namespace GraphTool
{

	public class Plotter : GraphPartsBase
	{
		const int MIN_DRAWLIMIT = 2;
		const string SHADER_NAME = "GraphTool/Plotter";

		[Header("Plotter"), GraphDataKey("handler")]
		public int dataKey = -1;

		[Header("Plot Option")]
		public float size = 1f;
		public bool drawLine = true;
		public bool cutoffDatalessFrame = true;

		[SerializeField, Range(MIN_DRAWLIMIT, 2000), Header("Load reduction")]
		private int drawsLimit = 100;

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			if (buffer != null && drawsLimit > buffer.count)
			{
				buffer.Dispose();
				CreateCapasityObject(drawsLimit);
			}
		}
#endif


		Mesh dummyMesh = null;
		ComputeBuffer buffer = null;
		PointData[] datas = null;

		public struct PointData
		{
			public Vector2 pos;
			public bool drawLine;
		}

		protected override void UpdateGeometry()
		{
			canvasRenderer.SetMesh(dummyMesh);
		}

		int ptsCount = 0;
		int prevFirst = 0;
		int prevLast = 0;
		protected override void OnUpdateGraph()
		{
			ptsCount = 0;

			if (dataKey == -1 || !handler.IsKeyValid(dataKey)) return;
			if (handler.InScopeFirstIndex == -1 || drawsLimit < MIN_DRAWLIMIT) return;

			int first = handler.InScopeFirstIndex;
			int last = handler.InScopeLastIndex;

			// Update Points
			if (prevFirst != first || prevLast != last)
			{
				var data = handler.GetDataReader(dataKey);
				var time = handler.GetDataReader(GraphHandler.SYSKEY_TIMESTAMP);

				int lim = drawsLimit;
				int len = last - first;

				Vector2? prevPoint = null;
				if (len >= lim)
				{
					// Skip
					int skip = (len + 1) / (lim - 1) + 1;
					first -= first % skip;

					// skip point
					for (int i= first; i + skip < last-1; i += skip)
						AddPointAverage(time, data, i, Mathf.Min(i + skip - 1, last), ref prevPoint);

					// last point
					if (data[last] != null)
						AddPoint(time[last].Value, data[last].Value, ref prevPoint);
				}
				else
				{
					// No Skip
					for (int i = first; i <= last; ++i)
					{
						if (data[i] == null)
						{
							if (cutoffDatalessFrame) prevPoint = null;
							continue;
						}
						AddPoint(time[i].Value, data[i].Value, ref prevPoint);
					}
				}

				buffer.SetData(datas);
				prevFirst = first;
				prevLast = last;
			}

			// Update Material
			if (material != null)
			{
				material.SetBuffer("Points", buffer);
				material.SetInt("_PtsCount", ptsCount);
				material.SetFloat("_Scale", size);
				material.SetColor("_Color", color);

				var scope2Local =
					Matrix4x4.TRS(
						Vector2.Scale(transration, scale) + offset,
						Quaternion.identity,
						scale);

				material.SetMatrix("_S2LMatrix", scope2Local);

				var local2World = rectTransform.localToWorldMatrix;
				material.SetMatrix("_L2WMatrix", local2World);
			}

		}

		void AddPoint(float time, float data, ref Vector2? prevPoint)
		{
			var point = new Vector2(time, data);
			try
			{
				datas[ptsCount] = new PointData()
				{
					pos = point,
					drawLine = !cutoffDatalessFrame & drawLine
				};
			}
			catch (System.IndexOutOfRangeException e)
			{
				Debug.LogError(e.Message);
			}
			if(prevPoint != null)
			{
				datas[ptsCount - 1].drawLine = drawLine;
			}
			prevPoint = point;
			ptsCount++;
		}

		void AddPointAverage(Data.Reader time, Data.Reader data, int start, int end, ref Vector2? prevPoint)
		{
			float timeave = 0f;
			float dataave = 0f;
			int datacnt = 0;
			for (int i = start; i <= end ; ++i)
			{
				if (data[i] == null) continue;
				dataave += data[i].Value;
				timeave += time[i].Value;
				++datacnt;
			}

			if (datacnt != 0)
				AddPoint(timeave / datacnt, dataave / datacnt, ref prevPoint);
			else if (cutoffDatalessFrame)
				prevPoint = null;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			var shader = Shader.Find(SHADER_NAME);
			if (shader != null)
				material = new Material(shader);
			else
			{
				enabled = false;
				return;
			}

			CreateCapasityObject(drawsLimit);
		}

		void CreateCapasityObject(int capasity)
		{
			dummyMesh = new Mesh();
			var vertices = new Vector3[capasity*3];
			var indices = new int[capasity*3];
			int i=0;
			for (; i<capasity*3; i++)
			{
				vertices[i] = new Vector3(i / 2, -i % 2) * 10;
				indices[i] = i;
			}
			dummyMesh.vertices = vertices;
			dummyMesh.SetIndices(indices, MeshTopology.Lines, 0);

			buffer = new ComputeBuffer(capasity, Marshal.SizeOf(typeof(PointData)));
			datas = new PointData[capasity];
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			if(buffer != null)
			{
				buffer.Release();
				buffer = null;
			}
		}
		
	}
	
}