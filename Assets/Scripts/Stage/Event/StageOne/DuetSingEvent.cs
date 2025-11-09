using Script.Manager;

namespace Script.Stage.Event.StageOne
{
    public class DuetSingEvent : StageEvent
    {
        public override StageEvent Initialize()
        {
            title = "듀엣 하모니!";
            description = "팀원과 완벽한 화음을 맞출 수 있을까?" +
                          "\n 주사위 눈금 2,4,6 일 경우 노래 + 주사위 눈금 * 2 " +
                          "\n 주사위 눈금 1,3,5 일 경우 아무일도 없다 ";
            dropSlotCount = 1;

            return this;
        }

        public override void EventAction(int diceEye)
        {
            if (diceEye == 2 || diceEye == 4 || diceEye == 6)
            {
                AllCharacterManager.Manager.Player.Stat.Sing.AddValue(diceEye * 2);
            }
        }
    }
}