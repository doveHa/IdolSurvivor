using Script.Manager;

namespace Script.Stage.Event.TitleStage
{
    public class MinusVoteEvent : StageEvent
    {
        public override StageEvent Initialize()
        {
            title = "앗차차";
            description = "무대에서 사소한 실수를 범했다." +
                          "\n투표 수 - 주사위 눈금";
            dropSlotCount = 1;

            return this;
        }

        public override void EventAction(int diceEye)
        {
            AllCharacterManager.Manager.Player.AddVote(-diceEye);
        }
    }
}