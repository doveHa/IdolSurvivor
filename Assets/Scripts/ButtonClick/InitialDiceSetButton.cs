using System;
using UnityEngine;
using UnityEngine.UI;

namespace Script.ButtonClick
{
    public class InitialDiceSetButton : ButtonOnClick
    {
        [SerializeField] private Button stageStartButton;

        protected override void OnClick()
        {
            DiceRoller.RollDice(DiceRoller.SIX_DICE_EYE);
            GetComponent<Button>().interactable = false;
        }

        void OnDisable()
        {
            stageStartButton.gameObject.SetActive(true);
        }
    }
}