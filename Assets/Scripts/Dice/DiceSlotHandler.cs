using Script.Stage;
using Script.Stage.Event;
using Script.UI.DragDrop;
using Script.UI.DragDrop.DropFunction;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class DiceSlotHandler : MonoBehaviour
    {
        [SerializeField] private GameObject slotPrefab;
        private GameObject[] slots;
        private int addedCount;

        void Start()
        {
            addedCount = 0;
        }

        public void CreateSlots()
        {
            slots = new GameObject[Config.Event.EventCount];

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = Instantiate(slotPrefab, transform);
            }
        }

        public void AddDraggableScript()
        {
            foreach (GameObject slot in slots)
            {
                slot.AddComponent<EventDiceDrop>();
                slot.AddComponent<DraggableObject>().CanDrag = true;
            }
        }

        public void AddDice(int dice)
        {
            slots[addedCount++].GetComponentInChildren<TextMeshProUGUI>().text = dice.ToString();
        }

        public void UseDice(int dice)
        {
            foreach (GameObject slot in slots)
            {
                DraggableObject dragObject = slot.GetComponent<DraggableObject>();
                if (dragObject != null && dragObject.CanDrag &&
                    int.Parse(slot.GetComponentInChildren<TextMeshProUGUI>().text) == dice)
                {
                    dragObject.MoveOriginalSpot();
                    dragObject.CanDrag = false;
                    dragObject.GetComponent<Image>().color = Color.gray;
                    return;
                }
            }
        }
    }
}