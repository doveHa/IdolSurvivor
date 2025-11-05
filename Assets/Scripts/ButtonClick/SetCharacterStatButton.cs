using System;
using System.Collections.Generic;
using Script.DataDefinition.Enum;
using Script.UI.DragDrop;
using Script.UI.DragDrop.DropFunction;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Script.ButtonClick
{
    public class SetCharacterStatButton : ButtonOnClick
    {
        [SerializeField] private GameObject[] dices;
        private List<int> diceEyes;
        private int currentCount;

        void Awake()
        {
            diceEyes = new List<int>();
        }

        protected override void OnClick()
        {
            DiceRoller.RollDice(6);
            diceEyes.Add(DiceRoller.lastRollResult);
            GameObject dice = dices[currentCount++];
            dice.GetComponent<DraggableObject>().CanDrag();
            dice.GetComponentInChildren<TextMeshProUGUI>().text = DiceRoller.lastRollResult.ToString();

            if (currentCount >= Constant.NUMBER_OF_STAT_TYPE)
            {
                GetComponent<UnityEngine.UI.Button>().interactable = false;
            }
        }
    }
}