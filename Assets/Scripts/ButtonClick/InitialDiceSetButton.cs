using UnityEngine.UI;

namespace Script.ButtonClick
{
    public class InitialDiceSetButton : ButtonOnClick
    {
        protected override void OnClick()
        {
            DiceRoller.RollDice(DiceRoller.SIX_DICE_EYE);
            GetComponent<Button>().interactable = false;
        }
    }
}