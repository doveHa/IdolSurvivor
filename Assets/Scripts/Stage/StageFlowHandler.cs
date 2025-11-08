using UnityEngine;
using UnityEngine.Serialization;

namespace Script.Stage
{
    public class StageFlowHandler : MonoBehaviour
    {
        [SerializeField] public DiceSlotHandler DiceSlot;

        void Start()
        {
            DiceSlot.CreateSlots();
            StartStage();
        }

        void StartStage()
        {
            StartCoroutine(DiceManager.Manager.DiceRoll());
        }
    }
}