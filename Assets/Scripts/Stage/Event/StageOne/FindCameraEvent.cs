using Script.Manager;

namespace Script.Stage.Event.StageOne
{
    public class FindCameraEvent : StageEvent
    {
        public override StageEvent Initialize()
        {
            title = "카메라 발견!";
            description = "무대 카메라가 자신을 잡았다! 눈빛으로 매력을 어필하자!! " +
                          "\n 주사위 눈금 5 이상 시 매력 + 주사위 눈금 x 2 " +
                          "\n 주사위 눈금 3,4 일 경우 매력 + 주사위 눈금 " +
                          "\n 주사위 눈금 1,2 일 경우 매력 - 주사위 눈금";
            dropSlotCount = 1;

            return this;
        }

        public override void EventAction(int diceEye)
        {
            if (diceEye >= 5)
            {
                AllCharacterManager.Manager.Player.Stat.Charm.AddValue(diceEye * 2);
            }
            else if (diceEye >= 3)
            {
                AllCharacterManager.Manager.Player.Stat.Charm.AddValue(diceEye);
            }
            else
            {
                AllCharacterManager.Manager.Player.Stat.Charm.AddValue(-diceEye);
            }
        }
        
    }
}