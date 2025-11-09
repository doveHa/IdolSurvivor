using Script.Manager;

namespace Script.Stage.Event.StageOne
{
    public class SpotLightPoseEvent : StageEvent
    {
        public override StageEvent Initialize()
        {
            title = "스포트라이트 포즈";
            description = "조명 아래에서 자신만의 포즈로 무대를 장식하자!! " +
                          "\n 주사위 눈금 3 이상 시 외모 + 주사위 눈금  " +
                          "\n 주사위 눈금 3 미만 일 경우 외모 - 주사위 눈금 ";
            dropSlotCount = 1;

            return this;
        }

        public override void EventAction(int diceEye)
        {
            if (diceEye >= 3)
            {
                AllCharacterManager.Manager.Player.Stat.Appearance.AddValue(diceEye);
            }
            else
            {
                AllCharacterManager.Manager.Player.Stat.Appearance.AddValue(-diceEye);
            }
        }
    }
}