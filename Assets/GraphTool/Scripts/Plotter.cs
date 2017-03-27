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

	public class Plotter : GraphPartsBase
	{
		[Header("Plotter"), GraphDataKey("handler")]
		public int dataKey = -1;

		[Header("Plot Option")]
		public float size = 1f;
		public bool drawLine = true;
		public bool cutoffDatalessFrame = true;

		[SerializeField, Range(0, 2000), Header("Load reduction")]
		private int drawsLimit = 100;
		[SerializeField, Range(0, 500)]
		private int freshDataProtection = 10;

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
			freshDataProtection = Mathf.Min(freshDataProtection, drawsLimit / 3);

			if (buffer != null && drawsLimit > buffer.count)
			{
				buffer.Dispose();
				CreateComputeBuffer(drawsLimit);
			}
		}
#endif

		Mesh dummyMesh = null;
		ComputeBuffer buffer = null;
		Material proceduralMat = null;
		PointData[] datas = null;

		public struct PointData
		{
			public Vector2 pos;
			public bool drawLine;
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

			int ptc = Mathf.Max(0 , freshDataProtection - (data.Count - last));
			int lim = drawsLimit;
			int len = last - first;
			int skip = 1;
			if (len > lim)
			{
				int fdp2 = freshDataProtection * 2;
				skip += (len - fdp2) / ( lim - fdp2);
				first -= first % skip;
			}

			int i = first;
			try
			{
				
				Vector2? prevPoint = null;
				if (skip > 1)
				{
					for (;ptc > 0 ? (i + skip < last - ptc) : (i < last + skip); i += skip)
					{
						float timeave = 0f;
						float dataave = 0f;
						int datacnt = 0;
						for (int j = i; j < i + skip && j < data.Count; ++j)
						{
							if (data[j] != null)
							{
								dataave += data[j].Value;
								timeave += time[j].Value;
								++datacnt;
							}
						}
						if (datacnt == 0)
						{
							if (cutoffDatalessFrame) prevPoint = null;
							continue;
						}

						AddPoint(timeave / datacnt, dataave / datacnt, ref prevPoint);
					}
				}

				for (; i < last; ++i)
				{
					if (data[i] == null)
					{
						if (cutoffDatalessFrame) prevPoint = null;
						continue;
					}

					AddPoint(time[i].Value, data[i].Value, ref prevPoint);
				}

				buffer.SetData(datas);
			}
			catch (System.IndexOutOfRangeException)
			{
				int fdp2 = freshDataProtection * 2;
				Debug.LogErrorFormat("IndexOutOfRangeException" +
					"\nlim={0}, length={1}, skip={2}, ptc={3}" +
					"\ncount={4}, i={5}, tes={6}",
					lim, len, skip, ptc, ptsCount, i, (last - first - fdp2 + skip) / skip + fdp2 - 1);
			}
			
		}

		void AddPoint(float time, float data, ref Vector2? prevPoint)
		{
			//var point = ScopeToRect(new Vector2(time, data));
			var point = new Vector2(time, data);
			datas[ptsCount] = new PointData()
			{
				pos = point,
				drawLine = !cutoffDatalessFrame & drawLine
			};
			if(prevPoint != null)
			{
				datas[ptsCount - 1].drawLine = drawLine;
			}
			prevPoint = point;
			ptsCount++;
		}

		private void OnRenderObject()
		{
			RecalculateScale();

			if (handler == null || proceduralMat == null || ptsCount <= 0) return;

			proceduralMat.SetPass(0);
			proceduralMat.SetBuffer("Points", buffer);
			proceduralMat.SetFloat("_Scale", size);
			proceduralMat.SetColor("_Color", color);

			var matrix = Matrix4x4.TRS(Vector2.Scale( transration, scale) + offset, Quaternion.identity, scale);

			proceduralMat.SetMatrix("_S2LMatrix", matrix);
			proceduralMat.SetMatrix("_L2WMatrix", rectTransform.localToWorldMatrix);
			Graphics.DrawProcedural(MeshTopology.LineStrip, ptsCount);
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			var shader = Shader.Find("GraphTool/Plotter");
			if (shader != null)
				proceduralMat = new Material(shader);
			else
				return;

			CreateComputeBuffer(drawsLimit);
		}

		void CreateComputeBuffer(int capasity)
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