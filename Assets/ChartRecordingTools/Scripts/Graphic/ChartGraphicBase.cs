/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using UnityEngine;
using UnityEngine.UI;

namespace Sokuhatiku.ChartRecordingTools
{

	public abstract class ChartGraphicBase : MaskableGraphic, ICanNavigateToScope
	{
		[SerializeField, HideInInspector] protected Scope scope;
		public Scope GetScope()
		{
			return scope;
		}

		protected Vector2 Scope2RectTransration;
		protected Vector2 Scope2RectScale;
		protected Vector2 Scope2RectOffset;

#if UNITY_EDITOR
		protected override void Reset()
		{
			base.Reset();
			scope = GetComponentInParent<Scope>();
			if (scope != null)
				UpdateScope();
			
		}
#endif

		protected override void OnEnable()
		{
			base.OnEnable();
			scope = GetComponentInParent<Scope>();
			if(scope == null)
			{
				Debug.LogError("ChartGraphic must be below the Scope.");
				enabled = false;
				return;
			}

			UpdateScope();
			scope.OnUpdateScope += UpdateScope;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (scope != null)
			{
				scope.OnUpdateScope -= UpdateScope;
			}
		}
		
		protected void UpdateScope()
		{
			RecalculateScale();
			OnUpdateScope();
		}

		protected virtual void RecalculateScale()
		{
			var scopeRect = scope.ScopeRect;
			var tfRect = rectTransform.rect;
			var pivot = rectTransform.pivot;

			Scope2RectTransration = -scopeRect.position;
			Scope2RectScale = new Vector2(
				tfRect.width / scopeRect.width,
				tfRect.height / scopeRect.height);
			Scope2RectOffset = new Vector2(
				-pivot.x * tfRect.width,
				-pivot.y * tfRect.height);
		}

		protected abstract void OnUpdateScope();


		#region PointTranslators

		protected Vector2 ScopeToRect(Vector2 point)
		{
			point += Scope2RectTransration;
			point.Scale(Scope2RectScale);
			point += Scope2RectOffset;
			return point;
		}

		protected float ScopeToRectX(float x)
		{
			return (x + Scope2RectTransration.x) * Scope2RectScale.x + Scope2RectOffset.x;
		}

		protected float ScopeToRectY(float y)
		{
			return (y + Scope2RectTransration.y) * Scope2RectScale.y + Scope2RectOffset.y;
		}

		protected Vector2 RectToScope(Vector2 point)
		{
			point -= Scope2RectOffset;
			point = new Vector2(point.x / Scope2RectScale.x, point.y / Scope2RectScale.y);
			point -= Scope2RectTransration;
			return point;
		}

		protected float RectToScopeX(float x)
		{
			return (x - Scope2RectOffset.x) / Scope2RectScale.x - Scope2RectTransration.x;
		}

		protected float RectToScopeY(float y)
		{
			return (y - Scope2RectOffset.y) / Scope2RectScale.y - Scope2RectTransration.y;
		}

		#endregion

	}
}