using Script.Manager;

namespace Script.Stage.Event.TitleStage
{
    public class PlusVoteEvent : StageEvent
    {
        public override StageEvent Initialize()
        {
            title = "날 지켜봐줘!";
            description = "관객들을 사로잡아 투표 수를 늘린다!" +
                          "\n 투표 수 + 주사위 눈금";
            dropSlotCount = 1;

            return this;
        }

        public override void EventAction(int diceEye)
        {
            AllCharacterManager.Manager.Player.AddVote(diceEye);
        }
    }
}