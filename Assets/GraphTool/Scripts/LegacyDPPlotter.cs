/**
Graph Tool

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace GraphTool
{

	public class LegacyDPPlotter : GraphPartsBase
	{
		const string SHADER_NAME = "GraphTool/LegacyDPPlotter";
		[Header("Plotter"), GraphDataKey("handler")]
		public int dataKey = -1;

		[Header("Plot Option")]
		public float size = 1f;
		public bool drawLine = true;
		public bool cutoffDatalessFrame = true;

		[SerializeField, Range(5, 2000), Header("Load reduction")]
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

		ComputeBuffer buffer = null;
		Material proceduralMat = null;
		PointData[] datas = null;

		public struct PointData
		{
			public Vector2 pos;
			public bool drawLine;
		}

		protected override void UpdateGeometry()
		{
			//base.UpdateGeometry();
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
		}

		int ptsCount = 0;
		protected override void OnUpdateGraph()
		{
			ptsCount = 0;

			if (handler == null || dataKey == -1 || !handler.IsKeyValid(dataKey)) return;
			if (handler.InScopeFirstIndex == -1 || drawsLimit <= 0) return;

			var data = handler.GetDataReader(dataKey);
			var time = handler.GetDataReader(GraphHandler.SYSKEY_TIMESTAMP);

			int first = handler.InScopeFirstIndex;
			int last = handler.InScopeLastIndex;
			
			int lim = drawsLimit;
			int len = last - first;

			Vector2? prevPoint = null;
			if (len >= lim)
			{
				// Skip
				int skip = (len + 1) / (lim - 1 - 2) +1;
				first -= first % skip;
				last = Mathf.Min(data.Count-1, last - (last % skip) + skip);
				int i = first;

				// first point
				if (first == 0)
				{
					if (data[0] != null)
						AddPoint(time[0].Value, data[0].Value, ref prevPoint);
					AddPointAverage(time, data, 1, skip - 1, ref prevPoint);
					i += skip;
				}
				else AddPointAverage(time, data, i - skip, i - 1, ref prevPoint);

				// skip point
				for (; i+skip < last; i += skip)
					AddPointAverage(time, data, i, i+skip-1, ref prevPoint);

				// last point
				AddPointAverage(time, data, i,last-1 , ref prevPoint);
				if (data[last] != null)
					AddPoint(time[data.Count - 1].Value, data[data.Count - 1].Value, ref prevPoint);
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

		private void OnRenderObject()
		{
			RecalculateScale();

			if (handler == null || proceduralMat == null || ptsCount <= 0) return;

			proceduralMat.SetPass(0);
			proceduralMat.SetBuffer("Points", buffer);
			proceduralMat.SetFloat("_Scale", size);
			proceduralMat.SetColor("_Color", color);

			var scope2Local = 
				Matrix4x4.TRS(
					Vector2.Scale( transration, scale) + offset,
					Quaternion.identity,
					scale);

			proceduralMat.SetMatrix("_S2LMatrix", scope2Local);
			proceduralMat.SetMatrix("_L2WMatrix", rectTransform.localToWorldMatrix);
			Graphics.DrawProcedural(MeshTopology.LineStrip, ptsCount);
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			var shader = Shader.Find(SHADER_NAME);
			if (shader != null)
				proceduralMat = new Material(shader);
			else
				return;

			CreateCapasityObject(drawsLimit);
		}

		void CreateCapasityObject(int capasity)
		{
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

		public override Material GetModifiedMaterial(Material baseMaterial)
		{
			if (proceduralMat != null)
				proceduralMat = base.GetModifiedMaterial(proceduralMat);
			return base.GetModifiedMaterial(baseMaterial);
		}
	}
	
}