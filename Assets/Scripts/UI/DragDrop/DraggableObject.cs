using System.Collections.Generic;
using Script.UI.DragDrop.DropFunction;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.UI.DragDrop
{
    [RequireComponent(typeof(Collider2D), typeof(IDrop))]
    public class DraggableObject : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public bool CanDrag { get; set; }
        private Vector3 offset, originalPos;
        private bool isDragging;
        private RectTransform rectTransform;
        private Canvas canvas;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            CanDrag = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!CanDrag)
            {
                return;
            }

            GetComponent<IDrop>().Click();

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

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.transform.TryGetComponent(out DraggableObject draggableObject))
                {
                    draggableObject.MoveOriginalSpot();
                }

                if (result.gameObject.transform.TryGetComponent(out DroppableObject droppableObject))
                {
                    GetComponent<IDrop>().Drop(droppableObject);
                    return;
                }
            }

            MoveOriginalSpot();
        }

        public void MoveOriginalSpot()
        {
            transform.position = originalPos;
        }
    }
}