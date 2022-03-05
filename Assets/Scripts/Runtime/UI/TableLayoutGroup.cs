using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace SharedUnityMischief.UI
{
	[ExecuteAlways]
	public class TableLayoutGroup : LayoutGroup
	{
		[SerializeField] protected RectOffset _rowPadding = new RectOffset();
		[SerializeField] protected TextAnchor _cellAlignment = TextAnchor.UpperLeft;
		[SerializeField] protected float _spacingBetweenColumns = 0;
		[SerializeField] protected float _spacingBetweenRows = 0;
		[SerializeField] protected bool _forceExpandColumns = false;
		[SerializeField] protected bool _forceExpandRows = false;
		[SerializeField] protected bool _forceExpandCellWidth = true;
		[SerializeField] protected bool _forceExpandCellHeight = true;
		[SerializeField] protected bool _reverseColumns = false;
		[SerializeField] protected bool _reverseRows = false;
		private List<TableRow> _rows = new List<TableRow>();
		private int _numColumns = 0;

		public RectOffset rowPadding { get { return _rowPadding; } set { SetProperty(ref _rowPadding, value); } }
		public TextAnchor cellAlignment { get { return _cellAlignment; } set { SetProperty(ref _cellAlignment, value); } }
		public float spacingBetweenColumns { get { return _spacingBetweenColumns; } set { SetProperty(ref _spacingBetweenColumns, value); } }
		public float spacingBetweenRows { get { return _spacingBetweenRows; } set { SetProperty(ref _spacingBetweenRows, value); } }
		public bool forceExpandColumns { get { return _forceExpandColumns; } set { SetProperty(ref _forceExpandColumns, value); } }
		public bool forceExpandRows { get { return _forceExpandRows; } set { SetProperty(ref _forceExpandRows, value); } }
		public bool forceExpandCellWidth { get { return _forceExpandCellWidth; } set { SetProperty(ref _forceExpandCellWidth, value); } }
		public bool forceExpandCellHeight { get { return _forceExpandCellHeight; } set { SetProperty(ref _forceExpandCellHeight, value); } }
		public bool reverseColumns { get { return _reverseColumns; } set { SetProperty(ref _reverseColumns, value); } }
		public bool reverseRows { get { return _reverseRows; } set { SetProperty(ref _reverseRows, value); } }

		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();

			FindAllRowsAndCells();

			float totalMinWidth = padding.horizontal + _spacingBetweenColumns * (_numColumns - 1);
			float totalPreferredWidth = padding.horizontal + _spacingBetweenColumns * (_numColumns - 1);
			float totalFlexibleWidth = 0f;
			for (int c = 0; c < _numColumns; c++)
			{
				GetColumnWidths(c, out float minColumnWidth, out float preferredColumnWidth, out float flexibleColumnWidth);
				totalMinWidth += minColumnWidth;
				totalPreferredWidth += preferredColumnWidth;
				totalFlexibleWidth += flexibleColumnWidth;
			}
			SetLayoutInputForAxis(totalMinWidth + _rowPadding.horizontal, totalPreferredWidth + _rowPadding.horizontal, totalFlexibleWidth, 0);
		}

		public override void CalculateLayoutInputVertical()
		{
			float totalMinHeight = padding.vertical + _spacingBetweenRows * (_rows.Count - 1);
			float totalPreferredHeight = padding.vertical + _spacingBetweenRows * (_rows.Count - 1);
			float totalFlexibleHeight = 0f;
			for (int r = 0; r < _rows.Count; r++)
			{
				GetRowHeights(r, out float minRowHeight, out float preferredRowHeight, out float flexibleRowHeight);
				totalMinHeight += minRowHeight + _rowPadding.vertical;
				totalPreferredHeight += preferredRowHeight + _rowPadding.vertical;
				totalFlexibleHeight += flexibleRowHeight;
			}
			SetLayoutInputForAxis(totalMinHeight, totalPreferredHeight, totalFlexibleHeight, 1);
		}

		public override void SetLayoutHorizontal()
		{
			int startIndex = _reverseColumns ? _numColumns - 1 : 0;
			int endIndex = _reverseColumns ? 0 : _numColumns - 1;
			int increment = _reverseColumns ? -1 : 1;
			float startX = 0f;
			float itemFlexibleMultiplier = 0f;
			float surplusSpace = rectTransform.rect.width - GetTotalPreferredSize(0);

			if (surplusSpace > 0)
			{
				if (GetTotalFlexibleSize(0) == 0)
					startX = GetStartOffset(0, GetTotalPreferredSize(0) - padding.horizontal);
				else if (GetTotalFlexibleSize(0) > 0)
					itemFlexibleMultiplier = surplusSpace / GetTotalFlexibleSize(0);
			}

			float minMaxLerp = 0;
			if (GetTotalMinSize(0) != GetTotalPreferredSize(0))
				minMaxLerp = Mathf.Clamp01((rectTransform.rect.width - GetTotalMinSize(0)) / (GetTotalPreferredSize(0) - GetTotalMinSize(0)));

			float rowWidth = 0f;
			float cellX = _rowPadding.left;
			for (int c = startIndex; _reverseColumns ? c >= endIndex : c <= endIndex; c += increment)
			{
				GetColumnWidths(c, out float minColumnWidth, out float preferredColumnWidth, out float flexibleColumnWidth);
				float columnWidth = Mathf.Lerp(minColumnWidth, preferredColumnWidth, minMaxLerp);
				columnWidth += flexibleColumnWidth * itemFlexibleMultiplier;
				foreach (TableRow row in _rows)
				{
					if (c < row.cells.Count)
					{
						RectTransform cellRect = row.cells[c];
						float preferredCellWidth = LayoutUtility.GetPreferredWidth(cellRect);
						float cellWidth;
						if (forceExpandCellWidth)
							cellWidth = columnWidth;
						else
							cellWidth = Mathf.Min(preferredCellWidth, columnWidth);
						float surplusWidth = columnWidth - cellWidth;
						SetChildAlongAxisWithScale(row.cells[c], 0, cellX + surplusWidth * GetCellAlignmentOnAxis(0), cellWidth, 1f);
					}
				}
				cellX += columnWidth + (c == endIndex ? 0f : spacingBetweenColumns);
				rowWidth += columnWidth + (c == endIndex ? 0f : spacingBetweenColumns);
			}
			foreach (TableRow row in _rows)
			{
				SetChildAlongAxisWithScale(row.rect, 0, startX + padding.left, rowWidth + _rowPadding.horizontal, 1f);
			}
		}

		public override void SetLayoutVertical()
		{
			int startIndex = _reverseRows ? _rows.Count - 1 : 0;
			int endIndex = _reverseRows ? 0 : _rows.Count;
			int increment = _reverseRows ? -1 : 1;
			float startY = padding.top;
			float itemFlexibleMultiplier = 0;
			float surplusSpace = rectTransform.rect.height - GetTotalPreferredSize(1);

			if (surplusSpace > 0)
			{
				if (GetTotalFlexibleSize(1) == 0)
					startY = GetStartOffset(1, GetTotalPreferredSize(1) - padding.vertical);
				else if (GetTotalFlexibleSize(1) > 0)
					itemFlexibleMultiplier = surplusSpace / GetTotalFlexibleSize(1);
			}

			float minMaxLerp = 0;
			if (GetTotalMinSize(1) != GetTotalPreferredSize(1))
				minMaxLerp = Mathf.Clamp01((rectTransform.rect.height - GetTotalMinSize(1)) / (GetTotalPreferredSize(1) - GetTotalMinSize(1)));

			float rowY = startY;
			for (int r = startIndex; _reverseRows ? r >= endIndex : r < endIndex; r += increment)
			{
				GetRowHeights(r, out float minRowHeight, out float preferredRowHeight, out float flexibleRowHeight);
				float rowHeight = Mathf.Lerp(minRowHeight, preferredRowHeight, minMaxLerp);
				rowHeight += flexibleRowHeight * itemFlexibleMultiplier;
				SetChildAlongAxisWithScale(_rows[r].rect, 1, rowY, rowHeight + _rowPadding.vertical, 1f);
				foreach (RectTransform cellRect in _rows[r].cells)
				{
					float preferredCellHeight = LayoutUtility.GetPreferredHeight(cellRect);
					float cellHeight;
					if (forceExpandCellHeight)
						cellHeight = rowHeight;
					else
						cellHeight = Mathf.Min(preferredCellHeight, rowHeight);
					float surplusHeight = rowHeight - cellHeight;
					SetChildAlongAxisWithScale(cellRect, 1, _rowPadding.top + surplusHeight * GetCellAlignmentOnAxis(1), cellHeight, 1f);
				}
				rowY += rowHeight + _rowPadding.vertical + spacingBetweenRows;
			}
		}

		private void GetColumnWidths(int columnIndex, out float minColumnWidth, out float preferredColumnWidth, out float flexibleColumnWidth)
		{
			minColumnWidth = -1f;
			preferredColumnWidth = -1f;
			flexibleColumnWidth = -1f;
			for (int r = 0; r < _rows.Count; r++)
			{
				if (columnIndex < _rows[r].cells.Count)
				{
					RectTransform cellRect = _rows[r].cells[columnIndex];

					float minWidth = LayoutUtility.GetMinWidth(cellRect);
					float preferredWidth = LayoutUtility.GetPreferredWidth(cellRect);
					float flexibleWidth = LayoutUtility.GetFlexibleWidth(cellRect);

					minColumnWidth = Mathf.Max(minColumnWidth, minWidth);
					preferredColumnWidth = Mathf.Max(preferredColumnWidth, preferredWidth);
					flexibleColumnWidth = Mathf.Max(flexibleColumnWidth, flexibleWidth);

					if (_forceExpandColumns)
						flexibleColumnWidth = Mathf.Max(flexibleColumnWidth, 1f);
				}
			}
		}

		private void GetRowHeights(int rowIndex, out float minRowHeight, out float preferredRowHeight, out float flexibleRowHeight)
		{
			minRowHeight = -1f;
			preferredRowHeight = -1f;
			flexibleRowHeight = -1f;
			if (rowIndex < _rows.Count)
			{
				TableRow row = _rows[rowIndex];
				for (int c = 0; c < row.cells.Count; c++)
				{
					RectTransform cellRect = row.cells[c];

					float minHeight = LayoutUtility.GetMinHeight(cellRect);
					float preferredHeight = LayoutUtility.GetPreferredHeight(cellRect);
					float flexibleHeight = LayoutUtility.GetFlexibleHeight(cellRect);

					minRowHeight = Mathf.Max(minRowHeight, minHeight);
					preferredRowHeight = Mathf.Max(preferredRowHeight, preferredHeight);
					flexibleRowHeight = Mathf.Max(flexibleRowHeight, flexibleHeight);

					if (_forceExpandRows)
						flexibleRowHeight = Mathf.Max(flexibleRowHeight, 1f);
				}
				minRowHeight = Mathf.Max(minRowHeight, LayoutUtility.GetMinHeight(row.rect));
				preferredRowHeight = Mathf.Max(preferredRowHeight, LayoutUtility.GetPreferredHeight(row.rect));
				flexibleRowHeight = Mathf.Max(flexibleRowHeight, LayoutUtility.GetFlexibleHeight(row.rect));
			}
		}

		private void FindAllRowsAndCells()
		{
			_rows.Clear();
			_numColumns = 0;
			List<Component> toIgnoreList = ListPool<Component>.Get();
			for (int i = 0; i < rectTransform.childCount; i++)
			{
				RectTransform rowRect = rectTransform.GetChild(i) as RectTransform;
				if (!ShouldIgnoreRectTransform(rowRect, toIgnoreList))
				{
					List<RectTransform> cells = new List<RectTransform>();
					for (int j = 0; j < rowRect.childCount; j++)
					{
						RectTransform cellRect = rowRect.GetChild(j) as RectTransform;
						if (!ShouldIgnoreRectTransform(cellRect, toIgnoreList))
						{
							cells.Add(cellRect);
						}
					}
					_rows.Add(new TableRow
					{
						rect = rowRect,
						cells = cells
					});
					_numColumns = Mathf.Max(_numColumns, cells.Count);
				}
			}
			ListPool<Component>.Release(toIgnoreList);
		}

		private bool ShouldIgnoreRectTransform(RectTransform rect, List<Component> toIgnoreList = null)
		{
			if (rect == null || !rect.gameObject.activeInHierarchy)
			{
				return true;
			}
			else
			{
				if (toIgnoreList == null)
					toIgnoreList = new List<Component>();
				rect.GetComponents(typeof(ILayoutIgnorer), toIgnoreList);
				if (toIgnoreList.Count == 0)
				{
					return false;
				}
				else
				{
					for (int j = 0; j < toIgnoreList.Count; j++)
					{
						ILayoutIgnorer ignorer = (ILayoutIgnorer) toIgnoreList[j];
						if (!ignorer.ignoreLayout)
							return false;
					}
					return true;
				}
			}
		}

		private float GetCellAlignmentOnAxis(int axis)
		{
			if (axis == 0)
				return ((int)cellAlignment % 3) * 0.5f;
			else
				return ((int)cellAlignment / 3) * 0.5f;
		}

		private class TableRow
		{
			public RectTransform rect;
			public List<RectTransform> cells;
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

		protected override void Reset()
		{
			base.Reset();
		}
#endif
	}
}