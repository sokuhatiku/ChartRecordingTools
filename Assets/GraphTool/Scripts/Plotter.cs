/**
Graph Tool

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace GraphTool
{

	public class Plotter : GraphPartsBase
	{
		const int MIN_DRAWLIMIT = 3;
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

			if (dataKey > -1 &&
				handler.IsKeyValid(dataKey) &&
				handler.InScopeFirstIndex != -1 &&
				handler.InScopeLastIndex != -1)
			{

				int first = handler.InScopeFirstIndex;
				int last = handler.InScopeLastIndex;

				// Update Points
				if (prevFirst != first || prevLast != last)
				{
					ptsCount = 0;
					var data = handler.GetDataReader(dataKey);
					var time = handler.GetDataReader(GraphHandler.SYSKEY_TIMESTAMP);

					bool prevPoint = false;

					if (last - first >= drawsLimit)
					{
						// Skip
						int skip = Mathf.NextPowerOfTwo((last - first + drawsLimit - 7) / (drawsLimit - 5));
						var mask = skip - 1;

						first = Mathf.Max(0, first - (first&mask) - skip);
						last = Mathf.Min(time.Count-1, last - (last&mask) + skip*2);

						// first point
						if (first == 0)
						{
							AddPoint(time[0].Value, data[0], ref prevPoint);
						}

						// skip point
						for (int i = first; i + skip < last; i += skip)
							AddPointAverage(time, data, i, skip, ref prevPoint);

						// last point
						if (last == data.LatestIndex)
						{
							AddPoint(time[data.LatestIndex].Value, data.LatestValue, ref prevPoint);
						}

					}
					else
					{
						// No Skip
						for (int i = first; i <= last; ++i)
						{
							AddPoint(time[i].Value, data[i], ref prevPoint);
						}
					}

					buffer.SetData(datas);
					prevFirst = handler.InScopeFirstIndex;
					prevLast = handler.InScopeLastIndex;
				}
			}
			else
			{
				ptsCount = 0;
			}

			// Update Material
			if (material != null)
			{
				material.SetBuffer("Points", buffer);
				material.SetInt("_PointsCount", ptsCount);

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
				
				var maskRect = rectTransform.rect;
				material.SetVector("_ClippingRect", new Vector4(maskRect.xMin, maskRect.yMin, maskRect.xMax, maskRect.yMax));
				
			}

		}

		bool AddPoint(float time, float? data, ref bool connectPrev)
		{
			if (data == null)
			{
				if (cutoffDatalessFrame) connectPrev = false;
				return false;
			}
			var point = new Vector2(time, data.Value);
			datas[ptsCount] = new PointData
			{
				pos = point,
				drawLine = connectPrev,
			};
			ptsCount++;
			connectPrev = true;
			return true;
		}

		int AddPointAverage(Data.Reader time, Data.Reader data,
			int start, int dirAndCount, ref bool connectPrev)
		{
			var dir = System.Math.Sign(dirAndCount);
			if (dir == 0) return 0;
			var lim = start + dirAndCount;

			float timeave = 0f;
			float dataave = 0f;
			int datacnt = 0;

			start = Mathf.Clamp(start, 0, data.Count - 1);
			lim = Mathf.Clamp(lim, -1, data.Count);

			for (int i = start; dir > 0 ? i < lim : i > lim; i += dir)
			{
				if (data[i] == null) continue;
				dataave += data[i].Value;
				timeave += time[i].Value;
				++datacnt;
			}

			if (datacnt != 0)
			{
				AddPoint(timeave / datacnt, dataave / datacnt, ref connectPrev);
				return datacnt;
			}
			if (cutoffDatalessFrame)
				connectPrev = false;

			return 0;
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
			dummyMesh.SetIndices(indices, MeshTopology.Points, 0);

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

		//private void OnGUI()
		//{
		//	GUILayout.Label(string.Format("ptsCount:{0}\nFirst:{1}\nLast:{2}",  ptsCount, handler.InScopeFirstIndex, handler.InScopeLastIndex));
		//}

	}
	
}