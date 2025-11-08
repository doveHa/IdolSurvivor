using Script.Manager;

namespace Script.Stage.Event.TitleStage
{
    public class MinusVoteEvent : StageEvent
    {
        public override void EventAction(int diceEye)
        {
            CharacterSelectManager.Manager.Player.AddVote(-diceEye);
        }
    }
}