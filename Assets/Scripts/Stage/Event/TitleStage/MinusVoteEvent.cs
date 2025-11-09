using Script.Manager;

namespace Script.Stage.Event.TitleStage
{
    public class MinusVoteEvent : StageEvent
    {
        public override StageEvent Initialize()
        {
            title = "";
            description = "";
            dropSlotCount = 1;
            
            return this;
        }

        public override void EventAction(int diceEye)
        {
            AllCharacterManager.Manager.Player.AddVote(-diceEye);
        }
    }
}