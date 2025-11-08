namespace Script.Stage.Event
{
    public abstract class StageEvent
    {
        public string title;
        public string description;
        public int dropSlotCount;

        public abstract StageEvent Initialize();
        public abstract void EventAction(int diceEye);
    }
}