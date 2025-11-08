namespace Script.Stage.Event
{
    public abstract class StageEvent
    {
        public string title;
        public string description;

        public abstract void EventAction(int diceEye);
    }
}