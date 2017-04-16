/**
ChartRecordingTools

Copyright (c) 2017 Sokuhatiku

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using UnityEngine;
using System;

namespace Sokuhatiku.ChartRecordingTools
{

	[DisallowMultipleComponent, AddComponentMenu("ChartRecordingTools/Scope")]
	public class Scope : MonoBehaviour, ICanNavigateToRecorder
	{

		const float DENOMINATOR_MIN = 0.01f;


		[SerializeField]
		Recorder recorder;
		public Recorder GetRecorder()
		{
			return recorder;
		}

		[SerializeField]
		protected bool Unsigned = false;
		public void ToggleUnsignedScope(bool toggle)
		{
			Unsigned = toggle;
		}

		[SerializeField]
		protected bool FollowLatest = true;
		public void ToggleFollowingLatest(bool toggle)
		{
			FollowLatest = toggle;
		}

		[SerializeField]
		protected Vector2 _offset = new Vector2(0, 0);
		public Vector2 Offset
		{
			get { return _offset; }
			set {_offset = value; }
		}
		Vector2 DefaultOffset;

		[SerializeField]
		protected Vector2 _size = new Vector2(10f, 20f);
		public Vector2 Size
		{
			get { return _size; }
			set { _size = DenominatorVector(value); }
		}
		Vector2 DefaultSize;

		[SerializeField]
		protected Vector2 _gridCellSize = new Vector2(1, 10);
		public Vector2 GridCellSize
		{
			get { return _gridCellSize; }
			set { _gridCellSize = DenominatorVector(value); }
		}

		[SerializeField]
		protected int _gridSubdivisionX = 4;
		public int GridSubdivisionX
		{
			get { return _gridSubdivisionX; }
			set { _gridSubdivisionX = Mathf.Max(value, 1); }
		}

		[SerializeField]
		protected int _gridSubdivisionY = 4;
		public int GridSubdivisionY
		{
			get { return _gridSubdivisionY; }
			set { _gridSubdivisionY = Mathf.Max(value, 1); }
		}
		

		public Action OnUpdateScope;
		
		public Rect ScopeRect{ get; private set; }

		protected int _inScopeFirstIndex = -1;
		public int InScopeFirstIndex { get { return _inScopeFirstIndex; } }

		protected int _inScopeLastIndex = -1;
		public int InScopeLastIndex { get { return _inScopeLastIndex; } }

		bool scopeIsDirty;

		public void MarkScopeDirty()
		{
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying)
			{
				UpdateScope();
				scopeIsDirty = false;
				return;
			}
#endif
			scopeIsDirty = true;
		}

		public void SetRecorder(Recorder newRecorder)
		{
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying)
			{
				recorder = newRecorder;
				return;
			}
#endif
			if(recorder != null)
			{
				recorder.OnUpdateData -= UpdateScope;
			}

			recorder = newRecorder;

			if(newRecorder != null)
			{
				newRecorder.OnUpdateData += UpdateScope;
			}
		}

		public void ResetScope()
		{
			_size = DefaultSize;
			_offset = DefaultOffset;
			MarkScopeDirty();
		}

		protected void UpdateScope()
		{
			if (FollowLatest && recorder != null)
				_offset.x = recorder.GetTimeline().LatestValue ?? _offset.x;

			var newScope = new Rect(_offset, _size);
			newScope.x -= _size.x;
			if (!Unsigned) newScope.y -= _size.y / 2;

			ScopeRect = newScope;

			UpdateIndexRange();

			if (OnUpdateScope != null)
				OnUpdateScope();
		}
		
		void UpdateIndexRange()
		{
			_inScopeFirstIndex = _inScopeLastIndex = -1;

			if (recorder == null) return;

			var time = recorder.GetTimeline();

			if (time.Count == 0)
			{
				return;
			}
			else if (time.Count == 1)
			{
				if (ScopeRect.xMin <= time[0].Value && time[0].Value <= ScopeRect.xMax)
					_inScopeFirstIndex = _inScopeLastIndex = 0;
				return;
			}
			else
			{
				if (time[time.Count - 1].Value < ScopeRect.xMin ||
					ScopeRect.xMax < time[0].Value)
					return;
				int fmin = 0, fmax = time.Count - 1;
				int lmin = 0, lmax = fmax;
				if (ScopeRect.xMin <= time[0].Value)
					_inScopeFirstIndex = 0;
				else
				{
					for (int i = 0; i < time.Count; i++)
					{
						var index = fmin + (fmax - fmin) / 2;
						if (time[index].Value < ScopeRect.xMin)
						{
							if (ScopeRect.xMin <= time[index + 1].Value)
							{
								_inScopeFirstIndex = index;
								break;
							}
							else
							{
								fmin = index + 1;
								lmin = Mathf.Max(lmin, fmin);
							}
						}
						else
						{
							fmax = index - 1;
							if (ScopeRect.xMax < time[index].Value)
								lmax = Mathf.Min(lmax, index);
						}
					}
				}
				if (time[time.Count - 1].Value <= ScopeRect.xMax)
					_inScopeLastIndex = time.Count - 1;
				else
				{
					for (int i = 0; i < time.Count; i++)
					{
						var index = lmin + (lmax - lmin) / 2;
						if (ScopeRect.xMax < time[index].Value)
						{
							if (time[index - 1].Value <= ScopeRect.xMax)
							{
								_inScopeLastIndex = index;
								break;
							}
							else
							{
								lmax = index - 1;
							}
						}
						else
						{
							lmin = index + 1;
						}
					}
				}
			}
		}
		

		Vector2 DenominatorVector(Vector2 vector)
		{
			return new Vector2(
				Mathf.Max(vector.x, DENOMINATOR_MIN),
				Mathf.Max(vector.y, DENOMINATOR_MIN));
		}


		void Reset()
		{
			recorder = GetComponentInParent<Recorder>();
		}

		private void OnValidate()
		{
			_size = new Vector2(Mathf.Max(_size.x, DENOMINATOR_MIN), Mathf.Max(_size.y, DENOMINATOR_MIN));
			_gridCellSize = new Vector2(Mathf.Max(_gridCellSize.x, DENOMINATOR_MIN), Mathf.Max(_gridCellSize.y, DENOMINATOR_MIN));
			_gridSubdivisionX = Mathf.Max(1, _gridSubdivisionX);
			_gridSubdivisionY = Mathf.Max(1, _gridSubdivisionY);

			MarkScopeDirty();
		}

		private void OnEnable()
		{
			if(recorder != null)
			{
				recorder.OnUpdateData += UpdateScope;
			}
			MarkScopeDirty();
		}

		private void OnDisable()
		{
			if(recorder != null)
			{
				recorder.OnUpdateData -= UpdateScope;
			}
		}

		private void Start()
		{
			DefaultOffset = Offset;
			DefaultSize = Size;
		}

		private void LateUpdate()
		{
			if(scopeIsDirty)
			{
				UpdateScope();
				scopeIsDirty = false;
			}
		}

	}
}
