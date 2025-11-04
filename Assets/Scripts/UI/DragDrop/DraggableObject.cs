using System.Collections.Generic;
using Script.UI.DragDrop.DropFunction;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.UI.DragDrop
{
    [RequireComponent(typeof(Collider2D))]
    public class DraggableObject : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private Vector3 offset, originalPos;
        private bool isDragging, canDrag;
        private RectTransform rectTransform;
        private Canvas canvas;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            canDrag = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!canDrag)
            {
                return;
            }

            Debug.Log("Click");

            isDragging = true;
            originalPos = transform.position;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out Vector2 mousePos
            );
            offset = rectTransform.localPosition - (Vector3)mousePos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging)
            {
                return;
            }

            Debug.Log("Dragging");
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out Vector2 mousePos
            );

            rectTransform.localPosition = (Vector3)mousePos + offset;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging)
            {
                return;
            }

            isDragging = false;
            Debug.Log("MouseUp");

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.transform.TryGetComponent(out DroppableObject draggableObject))
                {
                    GetComponent<IDrop>().Drop(draggableObject);
                    return;
                }
            }

            transform.position = originalPos;
        }

        public void CanDrag()
        {
            canDrag = true;
        }
    }
}