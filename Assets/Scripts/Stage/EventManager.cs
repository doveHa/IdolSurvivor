using Script.Manager;
using Script.Stage.Event;
using TMPro;
using UnityEngine;

namespace Script.Stage
{
    public class EventManager : MonoBehaviour
    {
        [SerializeField] private GameObject eventPanel;
        [SerializeField] private GameObject selectButton;

        [SerializeField] private Transform dropSlot;
        [SerializeField] private GameObject droppableSlotPrefab;

        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;

        public static EventManager Manager { get; private set; }
        public StageEvent CurrentEvent { private get; set; }
        public int OnDiceSlotEye { get; set; }

        void Awake()
        {
            if (Manager == null)
            {
                Manager = this;
            }
        }

        public void ShowEventPanel()
        {
            eventPanel.SetActive(true);
        }

        public void SetEventTexts()
        {
            title.text = CurrentEvent.title;
            description.text = CurrentEvent.description;
        }

        public void EndEvent()
        {
            for (int i = 0; i < dropSlot.childCount; i++)
            {
                Destroy(dropSlot.GetChild(i).gameObject);
            }

            eventPanel.SetActive(false);
        }

        public void CreateDropSlot()
        {
            Debug.Log(CurrentEvent.dropSlotCount);

            for (int i = 0; i < CurrentEvent.dropSlotCount; i++)
            {
                Instantiate(droppableSlotPrefab, dropSlot);
            }
        }

        public void AdjustEvent()
        {
            CurrentEvent.EventAction(OnDiceSlotEye);
            EndEvent();
        }

        public void ShowSelectButton()
        {
            selectButton.SetActive(true);
        }
    }
}