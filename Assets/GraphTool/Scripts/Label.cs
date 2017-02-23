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
		public float spacing = 1f;
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
			for(int i=0; i<10; ++i)
				generators.Add(new TextGenerator());
		}

		List<TextGenerator> generators;
		readonly UIVertex[] m_TempVerts = new UIVertex[4];
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (handler == null || font == null) return;

			var spacing = this.spacing;
			var Scope = handler.GetScope();
			var Tf = rectTransform.rect;
			var pivot = rectTransform.pivot;

			var drawCount = direction ? 
				Mathf.CeilToInt(Scope.width / spacing) + 1 :
				Mathf.CeilToInt(Scope.height / spacing) + 1;
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
			
			if (drawCount > generators.Count)
				drawCount = generators.Count;

			var setting = GetTextSetting();
			vh.Clear();
			for (int i= -Mathf.Epsilon < offset ? 0 : 1; i < drawCount; i++)
			{
				generators[i].Populate((spacing * (startNum + i)).ToString("#0.#"), setting);
				IList<UIVertex> verts = generators[i].verts;

				var vertexCount = verts.Count - 4;
				for (int v = 0; v < vertexCount; ++v)
				{
					int tempVertsIndex = v & 3;
					m_TempVerts[tempVertsIndex] = verts[v];
					m_TempVerts[tempVertsIndex].position += set + gain * i;
					if (tempVertsIndex == 3)
						vh.AddUIVertexQuad(m_TempVerts);
				}
			}
			
		}

		public int fontsize = 14;
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
			var scope = handler.GetScope();
			setting.textAnchor = anchor;
			setting.scaleFactor = 1;
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