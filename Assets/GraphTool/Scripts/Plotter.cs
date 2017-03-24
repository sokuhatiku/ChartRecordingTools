/**
Graph Tool

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace GraphTool
{

	public class Plotter : GraphPartsBase
	{
		[Header("Plotter"), GraphDataKey("handler")]
		public int dataKey = -1;

		[Header("Plot Option")]
		public bool drawDot = true;
		public float dotRadius = 1f;
		public float dotFloating = 0f;
		public bool drawLine = true;
		public float lineRadius = 0.25f;
		public bool CutoffDatalessFrame = true;

		[Range(10, 2000), Header("Load reduction")]
		public int drawsLimit = 100;
		[Range(0, 500)]
		public int freshDataProtection = 10;

		public readonly Vector2[] pointList = new Vector2[] {
			new Vector2(-5, 0),
			new Vector2(-4, 1),
			new Vector2(-3, 0),
			new Vector2(-2,-1),
			new Vector2(-1, 3),
			new Vector2( 0,-2),
			new Vector2( 1, 0),
		};



		public bool vertexhelper = true;

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			if (handler == null || !vertexhelper) return;
			RecalculateScale();

			Vector2? prevPoint = null;
			foreach (var p in pointList)
			{
				PlotToVH(vh, p.x, p.y, ref prevPoint);
			}
			return;

			//if (handler == null || dataKey == -1 || !handler.IsKeyValid(dataKey)) return;
			//if (handler.InScopeFirstIndex == -1) return;
			//var data = handler.GetDataReader(dataKey);
			//var time = handler.GetDataReader(GraphHandler.SYSKEY_TIMESTAMP);

			//var first = handler.InScopeFirstIndex;
			//var last = handler.InScopeLastIndex;
			//for (; last < data.Count && data[last] == null; ++last) { }
			//if (last == data.Count - 1 && data[last] == null)
			//	last = handler.InScopeLastIndex;

			//var draws = last - first;
			//var skip = draws > drawsLimit ? Mathf.CeilToInt(draws / drawsLimit) : 1;
			//first -= first % skip;

			//int i = first;
			//if (skip > 1)
			//{
			//	for (; i + skip + freshDataProtection < data.Count && i <= last + skip; i += skip)
			//	{
			//		float timeave = 0f;
			//		float dataave = 0f;
			//		int datacnt = 0;
			//		for (int j = i; 0 <= j && i - skip < j; --j)
			//		{
			//			if (data[j] != null)
			//			{
			//				dataave += data[j].Value;
			//				timeave += time[j].Value;
			//				++datacnt;
			//			}
			//		}
			//		if (datacnt == 0)
			//		{
			//			if (CutoffDatalessFrame) prevPoint = null;
			//			continue;
			//		}

			//		Plot(vh, timeave / datacnt, dataave / datacnt, ref prevPoint);
			//	}
			//}

			//for (; i < data.Count && i <= last; ++i)
			//{
			//	if (data[i] == null)
			//	{
			//		if (CutoffDatalessFrame) prevPoint = null;
			//		continue;
			//	}

			//	Plot(vh, time[i].Value, data[i].Value, ref prevPoint);
			//}

		}

		void PlotToVH(VertexHelper vh, float time, float data, ref Vector2? prevPoint)
		{
			var point = ScopeToRect(new Vector2(time, data));

			if (drawDot) AddDot(vh, new Vector3(point.x, point.y, dotFloating), dotRadius);
			if (drawLine && prevPoint != null) AddLine(vh, prevPoint.Value, point, lineRadius);

			prevPoint = point;
		}



		public bool computebuffer = true;

		ComputeBuffer buffer = null;
		Material proceduralMat = null;
		PointData[] datas = null;

		[Range(0, 10)]
		public float CB_Scale = 1f;

		public struct PointData
		{
			public Vector2 pos;
			public bool drawLine;
		}

		private void OnRenderObject()
		{
			RecalculateScale();

			if (handler == null || !computebuffer) return;

			Vector2? prevPoint = null;
			var ptcnt = 0;

			foreach(var p in pointList)
			{
				PlotToBuffer(ref ptcnt, p.x, p.y, ref prevPoint);
			}


			buffer.SetData(datas);
			proceduralMat.SetPass(0);
			proceduralMat.SetBuffer("Points", buffer);
			proceduralMat.SetFloat("_Scale", CB_Scale);
			proceduralMat.SetMatrix("_L2WMatrix", rectTransform.localToWorldMatrix);
			
			Graphics.DrawProcedural(MeshTopology.Points, pointList.Length);
		}

		void PlotToBuffer(ref int ptcnt, float time, float data, ref Vector2? prevPoint)
		{
			var point = ScopeToRect(new Vector2(time, data));
			datas[ptcnt] = new PointData()
			{
				pos = point,
				drawLine = prevPoint != null
			};
			ptcnt++;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			var shader = Shader.Find("GraphTool/Plotter");
			if (shader != null)
			{
				proceduralMat = new Material(shader);
			}
			else
				return;
			Debug.Log("CreateMat");

			buffer = new ComputeBuffer(100, Marshal.SizeOf(typeof(PointData)));
			Debug.Log("CreateBuffer");

			datas = new PointData[100];
			Debug.Log("CreateDatas");
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			if(buffer != null)
			{
				buffer.Release();
				buffer = null;
				Debug.Log("DisposeBuffer");
			}
		}
	}
	
}