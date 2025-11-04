using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Script.UI
{
    public class ButtonInputMotion : Button, IPointerDownHandler, IPointerUpHandler
    {
        private RectTransform childrenRect;
        [SerializeField] private float pressOffset = 10f;

        private Vector2 originalPos;

        private void Awake()
        {
            childrenRect = transform.GetChild(0).GetComponent<RectTransform>();
            originalPos = childrenRect.anchoredPosition;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (GetComponent<UnityEngine.UI.Button>().IsInteractable())
            {
                base.OnPointerDown(eventData);
                ApplyPressedVisual();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (GetComponent<UnityEngine.UI.Button>().IsInteractable())
            {
                base.OnPointerUp(eventData);
                ApplyNormalVisual();
            }
        }

        private void ApplyPressedVisual()
        {
            if (childrenRect != null)
                childrenRect.anchoredPosition = originalPos - new Vector2(0, pressOffset);
        }

        private void ApplyNormalVisual()
        {
            if (childrenRect != null)
                childrenRect.anchoredPosition = originalPos;
        }
    }
}