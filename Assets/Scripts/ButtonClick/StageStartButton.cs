using Script.Stage;
using UnityEngine;

namespace Script.ButtonClick
{
    public class StageStartButton : ButtonOnClick
    {
        [SerializeField] private GameObject initializeDicePanel;
        protected override void OnClick()
        {
            initializeDicePanel.SetActive(false);
            DiceManager.Manager.AddDraggableScript();
            DiceManager.Manager.HideDiceSlot();
            StageFlowManager.Manager.StageStart();
        }
    }
}