using System.Collections.Generic;
using Script.Manager;
using UnityEngine;

namespace Script.ButtonClick
{
    public class GradingButton : ButtonOnClick
    {
        [SerializeField] private GameObject Grade_A;
        [SerializeField] private GameObject Grade_B;
        [SerializeField] private GameObject Grade_C;

        private Dictionary<int, GameObject> panels;

        void Awake()
        {
            panels = new Dictionary<int, GameObject>();
            panels.Add(Constant.InitialVoteCount.GRADE_A, Grade_A);
            panels.Add(Constant.InitialVoteCount.GRADE_B, Grade_B);
            panels.Add(Constant.InitialVoteCount.GRADE_C, Grade_C);
        }

        protected override void OnClick()
        {
            DiceRoller.RollDice(6);
            switch (DiceRoller.lastRollResult)
            {
                case 1:
                case 2:
                case 3:
                    AdjustResult(Constant.InitialVoteCount.GRADE_C);
                    break;
                case 4:
                case 5:
                    AdjustResult(Constant.InitialVoteCount.GRADE_B);
                    break;
                case 6:
                    AdjustResult(Constant.InitialVoteCount.GRADE_A);
                    break;
            }
        }

        private void AdjustResult(int initialVoteCount)
        {
            CharacterSelectManager.Manager.Player.AddVote(initialVoteCount);
            foreach (KeyValuePair<int, GameObject> pair in panels)
            {
                if (pair.Key == initialVoteCount)
                {
                    //HighLight
                }
                else
                {
                    pair.Value.SetActive(false);
                }
            }
        }
    }
}