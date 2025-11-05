using System.Collections.Generic;
using Script.DataDefinition.Dialogue;
using Script.Manager;
using Script.UI;
using UnityEngine;

namespace Script.ButtonClick
{
    public class GradingButton : ButtonOnClick
    {
        [SerializeField] private GameObject Grade_A;
        [SerializeField] private GameObject Grade_B;
        [SerializeField] private GameObject Grade_C;

        [SerializeField] private GameObject ResultCanvas;
        private Dictionary<int, GameObject> panels;

        private const int GRADE_RESULT_POS_X = -600;
        private const int GRADE_RESULT_POS_Y = 40;

        private string dialogue;

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
                    dialogue = GradingDialogue.GRADE_C;
                    AdjustResult(Constant.InitialVoteCount.GRADE_C);
                    break;
                case 4:
                case 5:
                    dialogue = GradingDialogue.GRADE_B;
                    AdjustResult(Constant.InitialVoteCount.GRADE_B);
                    break;
                case 6:
                    dialogue = GradingDialogue.GRADE_A;
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
                    pair.Value.GetComponent<RectTransform>().localPosition =
                        new Vector3(GRADE_RESULT_POS_X, GRADE_RESULT_POS_Y, 0);
                    ResultCanvas.SetActive(true);
                    ResultCanvas.transform.GetComponentInChildren<DisplayStats>().SetText(dialogue);
                    ResultCanvas.transform.GetComponentInChildren<DisplayStats>().Display();
                    gameObject.SetActive(false);
                }
                else
                {
                    pair.Value.SetActive(false);
                }
            }
        }
    }
}