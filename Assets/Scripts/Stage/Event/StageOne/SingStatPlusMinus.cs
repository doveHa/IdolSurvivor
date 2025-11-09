using Script.Manager;

namespace Script.Stage.Event.StageOne
{
    public class SingStatPlusMinus : StageEvent
    {
        public override StageEvent Initialize()
        {
            title = "애드립 타이밍!";
            description = "기가막힌 애드립을 시도해보자!" +
                          "\n 주사위 눈금 6 일 경우 노래 + 주사위 눈금 * 2 " +
                          "\n 주사위 눈금 4 이상 시 노래 + 주사위 눈금  " +
                          "\n 주사위 눈금 4 미만 일 경우 노래 - 주사위 눈금 ";
            dropSlotCount = 1;

            return this;
        }

        public override void EventAction(int diceEye)
        {
            if (diceEye == 6)
            {
                AllCharacterManager.Manager.Player.Stat.Sing.AddValue(diceEye * 2);
            }
            else if (diceEye >= 4)
            {
                AllCharacterManager.Manager.Player.Stat.Sing.AddValue(diceEye);
            }
            else
            {
                AllCharacterManager.Manager.Player.Stat.Sing.AddValue(-diceEye);
            }
        }
    }
}