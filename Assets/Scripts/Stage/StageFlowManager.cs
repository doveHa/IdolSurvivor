using Script.Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Stage
{
    public class StageFlowManager :ManagerBase<StageFlowManager>
    {
        void Start()
        {
            DiceInitialSetting();
        }

        private void DiceInitialSetting()
        {
            StartCoroutine(DiceManager.Manager.InitialDiceSet());
        }

        public void StageStart()
        {
            StageManager.Manager.StageStart();
        }
    }
}