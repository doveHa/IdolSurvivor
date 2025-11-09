using Script.Manager;

namespace Script.Stage.Event.StageOne
{
    public class SoundDelayEvent : StageEvent
    {
        public override StageEvent Initialize()
        {
            title = "사운드 딜레이!";
            description = "늘어지는 노래에 맞춰 춤을 추자!" +
                          "\n 주사위 눈금 2,5 일 경우 춤 + 주사위 눈금 * 3 ";
            dropSlotCount = 1;

            return this;
        }

        public override void EventAction(int diceEye)
        {
            if (diceEye == 2 || diceEye == 5)
            {
                AllCharacterManager.Manager.Player.Stat.Dance.AddValue(diceEye * 3);
            }
        }
    }
}