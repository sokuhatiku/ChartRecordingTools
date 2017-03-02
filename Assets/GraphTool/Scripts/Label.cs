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
		const int generatorCreations = 16;
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
		

		List<TextGenerator> generators = new List<TextGenerator>();
		readonly UIVertex[] m_TempVerts = new UIVertex[4];
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (handler == null || font == null) return;

			if(generators.Count != handler.GridAutoScalingThresholdX + 2)
			{
				while (generators.Count < handler.GridAutoScalingThresholdX + 2)
					generators.Add(new TextGenerator());
				while (generators.Count > handler.GridAutoScalingThresholdX + 2)
					generators.RemoveAt(generators.Count - 1);
			}

			vh.Clear();
			var gridSize = direction ?
				handler.GridScale.x :
				handler.GridScale.y;
			if (gridSize < 0.01f) return;

			var subdivision = direction ?
				handler.GridSubdivisionX :
				handler.GridSubdivisionY;
			if (subdivision < 1) return;

			var width = direction ?
				handler.ScopeRect.width :
				handler.ScopeRect.height;

			var start = direction ?
				handler.ScopeRect.xMin :
				handler.ScopeRect.yMin;

			var draws = Mathf.CeilToInt(width / gridSize) +1;
			while (draws > handler.GridAutoScalingThresholdX)
			{ gridSize *= subdivision; draws = Mathf.CeilToInt(draws / subdivision); }
			draws += 2;

			var startNum = Mathf.FloorToInt(start / gridSize);
			var offset = -(start % gridSize);
			if (offset > 0) offset -= gridSize; // Graph shape to "/|/|/|/|/|"


			// convert to rectTransform position
			var tf_set = direction ?
				ScopeToRectX(start + offset) :
				ScopeToRectY(start + offset);
			var tf_gain = direction ?
				gridSize * scale.x :
				gridSize * scale.y;

			// draw
			var setting = GetTextSetting();
			var generatorID = 0;
			for (int i = 0; i < draws; ++i)
			{
				var translated = direction ? new Vector3(tf_set + tf_gain * i, (-rectTransform.pivot.y + 0.5f) * rectTransform.rect.height) :
					new Vector3((-rectTransform.pivot.x + 0.5f) * rectTransform.rect.width, tf_set + tf_gain * i);
				if (direction ?
					translated.x < rectTransform.rect.xMin - 1 || rectTransform.rect.xMax + 1 < translated.x :
					translated.y < rectTransform.rect.yMin - 1 || rectTransform.rect.yMax + 1 < translated.y) continue;

				generators[generatorID].Populate((gridSize * (startNum + i)).ToString("#0.#"), setting);
				IList<UIVertex> verts = generators[generatorID].verts;

				var vertexCount = verts.Count - 4;
				for (int v = 0; v < vertexCount; ++v)
				{
					int tempVertsIndex = v & 3;
					m_TempVerts[tempVertsIndex] = verts[v];
					m_TempVerts[tempVertsIndex].position += translated;
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