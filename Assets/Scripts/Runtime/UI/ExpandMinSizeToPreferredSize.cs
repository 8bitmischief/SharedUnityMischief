using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SharedUnityMischief.UI
{
	[RequireComponent(typeof(RectTransform))]
	[ExecuteAlways]
	public class ExpandMinSizeToPreferredSize : UIBehaviour, ILayoutElement
	{
		[SerializeField] private bool _expandMinWidth = true;
		[SerializeField] private bool _expandMinHeight = false;
		[SerializeField] private int _layoutPriority = 1;
		private ILayoutElement[] _layoutElements;

		public virtual void CalculateLayoutInputHorizontal()
		{
			_layoutElements = GetComponents<ILayoutElement>();
		}

		public virtual void CalculateLayoutInputVertical() {}

		public virtual float minWidth
		{
			get
			{
				if (_expandMinWidth)
				{
					float maxPreferredWidth = -1f;
					foreach (ILayoutElement element in _layoutElements)
						maxPreferredWidth = Mathf.Max(maxPreferredWidth, element.preferredWidth);
					return maxPreferredWidth;
				}
				else
				{
					return -1f;
				}
			}
		}

		public virtual float minHeight
		{
			get
			{
				if (_expandMinHeight)
				{
					float maxPreferredHeight = -1f;
					foreach (ILayoutElement element in _layoutElements)
						maxPreferredHeight = Mathf.Max(maxPreferredHeight, element.preferredHeight);
					return maxPreferredHeight;
				}
				else
				{
					return -1f;
				}
			}
		}

		public virtual float preferredWidth => -1;

		public virtual float preferredHeight => -1;

		public virtual float flexibleWidth => -1;

		public virtual float flexibleHeight => -1;

		public virtual int layoutPriority { get { return _layoutPriority; } set { if (SetStruct(ref _layoutPriority, value)) SetDirty(); } }

		protected ExpandMinSizeToPreferredSize() {}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetDirty();
		}

		protected override void OnTransformParentChanged()
		{
			SetDirty();
		}

		protected override void OnDisable()
		{
			SetDirty();
			base.OnDisable();
		}

		protected override void OnDidApplyAnimationProperties()
		{
			SetDirty();
		}

		protected override void OnBeforeTransformParentChanged()
		{
			SetDirty();
		}

		protected void SetDirty()
		{
			if (!IsActive())
				return;
			LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			SetDirty();
		}
#endif

		private static bool SetStruct<T>(ref T currentValue, T newValue) where T: struct
		{
			if (currentValue.Equals(newValue))
				return false;

			currentValue = newValue;
			return true;
		}
	}
}