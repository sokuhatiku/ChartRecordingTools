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
		const int COUNT_GENERATORS = 16;
		public bool direction;
		public Font font;
		public int fontSize = 14;
		public float scaleFacter = 1f;
		public TextAnchor anchor = TextAnchor.MiddleCenter;
		public Vector2 textOffset = Vector2.zero;

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

		protected override void OnUpdateGraph()
		{
			SetVerticesDirty();
		}

		List<TextGenerator> generators = new List<TextGenerator>();
		readonly UIVertex[] m_TempVerts = new UIVertex[4];
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (handler == null || font == null) return;

			if(generators.Count != COUNT_GENERATORS)
			{
				while (generators.Count < COUNT_GENERATORS)
					generators.Add(new TextGenerator());
				while (generators.Count > COUNT_GENERATORS)
					generators.RemoveAt(generators.Count - 1);
			}

			vh.Clear();

			var cellSize = direction ?
				handler.GridCellSize.x :
				handler.GridCellSize.y;
			if (cellSize < 0.01f) return;

			var subdivision = direction ?
				handler.GridSubdivisionX :
				handler.GridSubdivisionY;
			if (subdivision < 1) return;

			var scopeStart = direction ?
				handler.ScopeRect.xMin :
				handler.ScopeRect.yMin;

			var scopeEnd = direction ?
				handler.ScopeRect.xMax :
				handler.ScopeRect.yMax;

			while((scopeEnd - scopeStart) / cellSize > generators.Count)
				cellSize = cellSize * 2;

			var countStart = Mathf.FloorToInt(scopeStart / cellSize) +1;
			var countEnd = Mathf.FloorToInt(scopeEnd / cellSize) +1;

			var draws = countEnd - countStart;
			float offset = -(scopeStart % cellSize);
			if (offset < 0) offset += cellSize; // offset shape to "/|/|/|/|/|"
			if(offset == 0)
			{
				draws++;
				countStart--;
			}

			// convert to rectTransform position
			var tf_gain = direction ?
				cellSize * scale.x :
				cellSize * scale.y;
			var tf_set = (direction ?
				ScopeToRectX(scopeStart + offset) :
				ScopeToRectY(scopeStart + offset));

			// draw
			var setting = GetTextSetting();
			font.RequestCharactersInTexture("1234567890-", setting.fontSize, setting.fontStyle);
			var genCount = 0;
			for (int i = 0; i < draws && genCount < generators.Count; i++)
			{
				var translated = direction ? 
					new Vector3(tf_set + tf_gain * i, (-rectTransform.pivot.y + 0.5f) * rectTransform.rect.height) :
					new Vector3((-rectTransform.pivot.x + 0.5f) * rectTransform.rect.width, tf_set + tf_gain * i);

				generators[genCount].Populate((cellSize * (countStart + i)).ToString("#0.#"), setting);
				IList<UIVertex> verts = generators[genCount].verts;

				var vertexCount = verts.Count - 4;
				for (int v = 0; v < vertexCount; ++v)
				{
					int tempVertsIndex = v & 3;
					m_TempVerts[tempVertsIndex] = verts[v];
					m_TempVerts[tempVertsIndex].position += translated + (Vector3)textOffset;
					if (tempVertsIndex == 3)
						vh.AddUIVertexQuad(m_TempVerts);
				}
				genCount++;
			}

		}
		
		TextGenerationSettings GetTextSetting()
		{
			var setting = new TextGenerationSettings();
			var fontdata = FontData.defaultFontData;
			
			setting.generationExtents = rectTransform.rect.size;
			
			setting.fontSize = fontSize;
			setting.textAnchor = anchor;
			setting.scaleFactor = scaleFacter;
			setting.color = color;
			setting.font = font;
			setting.pivot = new Vector2(0.5f, 0.5f);
			setting.richText = false;
			setting.lineSpacing = 0;
			setting.fontStyle = FontStyle.Normal;
			setting.resizeTextForBestFit = false;
			setting.horizontalOverflow = HorizontalWrapMode.Overflow;
			setting.verticalOverflow = VerticalWrapMode.Overflow;

			return setting;

		}
		
	}

}