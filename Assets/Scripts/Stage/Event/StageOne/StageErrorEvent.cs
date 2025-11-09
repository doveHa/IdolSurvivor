using Script.Manager;

namespace Script.Stage.Event.StageOne
{
    public class StageErrorEvent : StageEvent
    {
        public override StageEvent Initialize()
        {
            title = "무대 장치 오작동!";
            description = "소품이 예상보다 빨리 나왔다! 안무를 조정하자! " +
                          "\n 주사위 눈금 3 이상 시 춤 + 주사위 눈금  " +
                          "\n 주사위 눈금 3 미만 일 경우 춤 - 주사위 눈금 ";
            dropSlotCount = 1;

            return this;
        }

        public override void EventAction(int diceEye)
        {
            if (diceEye >= 3)
            {
                AllCharacterManager.Manager.Player.Stat.Dance.AddValue(diceEye);
            }
            else
            {
                AllCharacterManager.Manager.Player.Stat.Dance.AddValue(-diceEye);
            }
        }
    }
}