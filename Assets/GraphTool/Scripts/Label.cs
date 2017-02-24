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
		const int generatorCreations = 11;
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

			generators = new List<TextGenerator>(generatorCreations);
			for(int i=0; i< generatorCreations; ++i)
				generators.Add(new TextGenerator());
		}

		List<TextGenerator> generators;
		readonly UIVertex[] m_TempVerts = new UIVertex[4];
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (handler == null || font == null) return;

			vh.Clear();
			var grid = direction ?
				handler.GridScale.x :
				handler.GridScale.y;
			if (grid <= Mathf.Epsilon) return; // Just in case

			var width = direction ?
				handler.ScopeRect.width :
				handler.ScopeRect.height;

			var pos = direction ?
				handler.ScopeRect.xMin :
				handler.ScopeRect.yMin;

			var draws = Mathf.CeilToInt(width / grid) +1;
			while (draws > generators.Count) { grid *= 2; draws = (draws >> 1) +1; }
			var offset = -(pos % grid);
			if (offset > 0) offset -= grid; // Graph shape to "/|/|/|/|/|"
			var startNum = Mathf.FloorToInt(pos / grid);


			// convert to rectTransform position
			var set = direction ?
				new Vector3(ScopeToRectX(pos + offset), (-rectTransform.pivot.y + 0.5f) * rectTransform.rect.height) :
				new Vector3((-rectTransform.pivot.x + 0.5f) * rectTransform.rect.width, ScopeToRectY(pos + offset));
			var gain = direction ?
				new Vector3(grid * scale.x, 0f) :
				new Vector3(0f, grid * scale.y) ;

			// draw
			var setting = GetTextSetting();
			var generatorID = 0;
			for (int i = -Mathf.Epsilon < offset ? 0 : 1; i < draws; ++i)
			{
				var transration = set + gain * i;
				if (direction ?
					transration.x < rectTransform.rect.xMin + 0.01f || rectTransform.rect.xMax + 0.01f < transration.x :
					transration.y < rectTransform.rect.yMin + 0.01f || rectTransform.rect.yMax + 0.01f < transration.y) continue;
				generators[generatorID].Populate((grid * (startNum + i)).ToString("#0.#"), setting);
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