using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SharedUnityMischief.UI
{
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[RequireComponent(typeof(RectTransform))]
	public class TableRowLayoutGroup : UIBehaviour, ILayoutElement, ILayoutGroup
	{
		[System.NonSerialized] private RectTransform _rect;
		protected RectTransform rectTransform
		{
			get
			{
				if (_rect == null)
					_rect = GetComponent<RectTransform>();
				return _rect;
			}
		}

		public virtual float minWidth => -1f;
		public virtual float preferredWidth => -1f;
		public virtual float flexibleWidth => -1f;
		public virtual float minHeight => -1f;
		public virtual float preferredHeight => -1f;
		public virtual float flexibleHeight => -1f;
		public virtual int layoutPriority => -1;

		public virtual void CalculateLayoutInputHorizontal() {}
		public virtual void CalculateLayoutInputVertical() {}
		public virtual void SetLayoutHorizontal() {}
		public virtual void SetLayoutVertical() {}

		protected TableRowLayoutGroup() {}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetDirty();
		}

		protected override void OnDisable()
		{
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			base.OnDisable();
		}

		protected override void OnDidApplyAnimationProperties()
		{
			SetDirty();
		}

		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			if (isRootLayoutGroup)
				SetDirty();
		}

		protected virtual void OnTransformChildrenChanged()
		{
			SetDirty();
		}

		protected void SetDirty()
		{
			if (!IsActive())
				return;

			if (!CanvasUpdateRegistry.IsRebuildingLayout())
				LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			else
				StartCoroutine(DelayedSetDirty(rectTransform));
		}

		private IEnumerator DelayedSetDirty(RectTransform rectTransform)
		{
			yield return null;
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
		}

		private bool isRootLayoutGroup
		{
			get
			{
				Transform parent = transform.parent;
				if (parent == null)
					return true;
				return transform.parent.GetComponent(typeof(ILayoutGroup)) == null;
			}
		}

#if UNITY_EDITOR
		private int _capacity = 10;
		private Vector2[] _sizes = new Vector2[10];

		protected virtual void Update()
		{
			if (Application.isPlaying)
				return;

			int count = transform.childCount;

			if (count > _capacity)
			{
				if (count > _capacity * 2)
					_capacity = count;
				else
					_capacity *= 2;

				_sizes = new Vector2[_capacity];
			}

			bool dirty = false;
			for (int i = 0; i < count; i++)
			{
				RectTransform t = transform.GetChild(i) as RectTransform;
				if (t != null && t.sizeDelta != _sizes[i])
				{
					dirty = true;
					_sizes[i] = t.sizeDelta;
				}
			}

			if (dirty)
				LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
		}

		protected override void OnValidate()
		{
			SetDirty();
		}

		protected override void Reset()
		{
			base.Reset();
		}
#endif
	}
}
