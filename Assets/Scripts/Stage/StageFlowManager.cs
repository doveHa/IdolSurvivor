using Script.Manager;
using Script.Stage.Event;
using UnityEngine;

namespace Script.Stage
{
    public class StageFlowManager : MonoBehaviour
    {
        public static StageFlowManager Manager;
        [SerializeField] private GameObject initialDicePanel;

        void Awake()
        {
            if (Manager == null)
            {
                Manager = this;
            }
        }

        void Start()
        {
            initialDicePanel.SetActive(true);
            DiceInitialSetting();
        }

        private void DiceInitialSetting()
        {
            StartCoroutine(DiceManager.Manager.InitialDiceSet());
        }

        public void StageStart()
        {
            StageManager.Manager.TimeStart();
        }

        public void EventOccured()
        {
            EventManager.Manager.CurrentEvent = Config.Event.GetEvent();
            EventManager.Manager.CreateDropSlot();
            EventManager.Manager.SetEventTexts();
            EventManager.Manager.ShowEventPanel();
            DiceManager.Manager.ShowDiceSlot();
        }
    }
}