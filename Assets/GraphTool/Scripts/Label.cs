/**
Graph Tool

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GraphTool
{

	public class Label : GraphPartsBase
	{
		public bool direction;
		public Font font;

		public override Texture mainTexture
		{
			get
			{
				if (font != null && font.material != null && font.material.mainTexture != null)
					return font.material.mainTexture;
				return base.mainTexture;
			}
		}


#if UNITY_EDITOR
		protected override void Reset()
		{
			base.Reset();
			font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		}
#endif

		protected override void OnEnable()
		{
			base.OnEnable();

			generators = new List<TextGenerator>(20);
			for(int i=0; i<20; ++i)
				generators.Add(new TextGenerator());
		}

		List<TextGenerator> generators;
		readonly UIVertex[] m_TempVerts = new UIVertex[4];
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (handler == null || font == null) return;

			vh.Clear();
			var spacing = direction ?
				handler.GridScale.x :
				handler.GridScale.y;
			var Scope = handler.ScopeRect;
			var Tf = rectTransform.rect;
			var pivot = rectTransform.pivot;

			var drawCount = direction ? 
				Mathf.CeilToInt(Scope.width / spacing) + 1:
				Mathf.CeilToInt(Scope.height / spacing) + 1;
			while(drawCount > generators.Count)
			{
				//Debug.Log(drawCount + " > " + generators.Count);
				spacing *= 2;
				drawCount /= 2;
			}

			var startNum = direction ?
				Mathf.FloorToInt(Scope.xMin / spacing) :
				Mathf.FloorToInt(Scope.yMin / spacing);
			float offset = direction ?
				-(Scope.xMin % spacing):
				-(Scope.yMin % spacing);
			if (offset > 0) offset -= spacing; // Graph: /|/|/|/|/|

			var set = direction ? 
				new Vector3(ScopeToRectX(Scope.xMin + offset), (-pivot.y + 0.5f) * Tf.height, 0) :
				new Vector3((-pivot.x + 0.5f) * Tf.width, ScopeToRectY(Scope.yMin + offset),  0) ;
			var gain = direction ?
				new Vector3(spacing * scale.x, 0f) :
				new Vector3(0f, spacing * scale.y) ;

			var generatorID = 0;

			var setting = GetTextSetting();
			for (int i= -Mathf.Epsilon < offset ? 0 : 1; i < drawCount; ++i)
			{
				var transration = set + gain * i;
				if (direction ?
					transration.x < Tf.xMin + 0.01f || Tf.xMax + 0.01f < transration.x :
					transration.y < Tf.yMin + 0.01f || Tf.yMax + 0.01f < transration.y) continue;
				generators[generatorID].Populate((spacing * (startNum + i)).ToString("#0.#"), setting);
				IList<UIVertex> verts = generators[generatorID].verts;

				var vertexCount = verts.Count - 4;
				for (int v = 0; v < vertexCount; ++v)
				{
					int tempVertsIndex = v & 3;
					m_TempVerts[tempVertsIndex] = verts[v];
					m_TempVerts[tempVertsIndex].position += transration;
					if (tempVertsIndex == 3)
						vh.AddUIVertexQuad(m_TempVerts);
				}
				generatorID++;
			}
			
		}

		public int fontsize = 14;
		public float scaleFacter = 1f;
		public TextAnchor anchor = TextAnchor.MiddleCenter;
		TextGenerationSettings GetTextSetting()
		{
			var setting = new TextGenerationSettings();
			var fontdata = FontData.defaultFontData;
			
			setting.generationExtents = rectTransform.rect.size;
			
			if (font != null && font.dynamic)
			{
				setting.fontSize = fontsize;
				setting.resizeTextMinSize = fontdata.minSize;
				setting.resizeTextMaxSize = fontdata.maxSize;

			}
			setting.textAnchor = anchor;
			setting.scaleFactor = scaleFacter;
			setting.color = color;
			setting.font = font;
			setting.pivot = new Vector2(0.5f, 0.5f);
			setting.richText = fontdata.richText;
			setting.lineSpacing = fontdata.lineSpacing;
			setting.fontStyle = fontdata.fontStyle;
			setting.resizeTextForBestFit = fontdata.bestFit;
			setting.horizontalOverflow = fontdata.horizontalOverflow;
			setting.verticalOverflow = fontdata.verticalOverflow;

			return setting;

		}
	}

}