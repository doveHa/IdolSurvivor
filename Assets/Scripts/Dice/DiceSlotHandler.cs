using Script.Manager;
using Script.Stage;
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
            slots = new GameObject[StageManager.Manager.CurrentStage.eventCount];
            addedCount = 0;
        }

        public void CreateSlots()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = Instantiate(slotPrefab, transform);
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
                if (int.Parse(slot.GetComponent<TextMeshProUGUI>().text) == dice)
                {
                    slot.GetComponent<Button>().interactable = false;
                }
            }
        }
    }
}