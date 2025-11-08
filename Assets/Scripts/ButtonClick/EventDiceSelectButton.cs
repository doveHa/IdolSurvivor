using Script.Stage;

namespace Script.ButtonClick
{
    public class EventDiceSelectButton : ButtonOnClick
    {
        protected override void OnClick()
        {
            DiceManager.Manager.DiceSlot.UseDice(EventManager.Manager.OnDiceSlotEye);
            EventManager.Manager.AdjustEvent();
            DiceManager.Manager.HideDiceSlot();
            StageManager.Manager.TimeStart();
        }
    }
}