using Script.Stage;

namespace Script
{
    public class OwnedDiceHandler
    {
        private const int ALLCOUNT = 0;
        private const int DICE_EYES = 7;
        private const int NONE = 0;
        private int[] OwnedDices;

        public OwnedDiceHandler()
        {
            OwnedDices = new int[DICE_EYES];
        }

        public void InputDice(int dice)
        {
            OwnedDices[ALLCOUNT]++;
            OwnedDices[dice]++;
            StageManager.Manager.StageFlow.DiceSlot.AddDice(dice);
        }

        public bool UseDice(int dice)
        {
            if (OwnedDices[dice] > NONE)
            {
                OwnedDices[ALLCOUNT]--;
                OwnedDices[dice]--;
                StageManager.Manager.StageFlow.DiceSlot.UseDice(dice);
                return true;
            }

            return false;
        }

        public int Count()
        {
            return OwnedDices[ALLCOUNT];
        }
    }
}