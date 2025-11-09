using Script.Manager;
using Script.Stage.Event;

namespace Script.Stage
{
    public class StageFlowManager : ManagerBase<StageFlowManager>
    {
        void Start()
        {
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