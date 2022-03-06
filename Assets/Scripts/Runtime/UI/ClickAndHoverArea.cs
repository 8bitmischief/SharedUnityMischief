using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SharedUnityMischief.UI
{
	public class ClickAndHoverArea : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public Action onClick;
		public Action onHoverStart;
		public Action onHoverEnd;

		public void OnPointerClick(PointerEventData pointerEventData)
		{
			onClick?.Invoke();
		}

		public void OnPointerEnter(PointerEventData pointerEventData)
		{
			onHoverStart?.Invoke();
		}

		public void OnPointerExit(PointerEventData pointerEventData)
		{
			onHoverEnd?.Invoke();
		}
	}
}