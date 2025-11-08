using Script.Stage;
using TMPro;
using UnityEngine;

namespace Script.UI.DragDrop.DropFunction
{
    public class EventDiceDrop : IDrop
    {
        public override void Drop(DroppableObject drop)
        {
            GetComponent<RectTransform>().transform.position = drop.GetComponent<RectTransform>().transform.position;
            EventManager.Manager.OnDiceSlotEye = int.Parse(GetComponentInChildren<TextMeshProUGUI>().text);
            EventManager.Manager.ShowSelectButton();
        }
    }
}