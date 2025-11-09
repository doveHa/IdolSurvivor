using Script.Manager;
using Script.Stage.Event;
using TMPro;
using UnityEngine;

namespace Script.Stage
{
    public class StageDialogManager : ManagerBase<StageDialogManager>
    {
        [SerializeField] private TextMeshProUGUI initialDiceSetDialog;

        void Start()
        {
            DialogInitialize();
        }

        private void DialogInitialize()
        {
            InitialDiceSettingDescription();
        }

        private void InitialDiceSettingDescription()
        {
            initialDiceSetDialog.text =
                $"\n 총 {Config.Event.EventCount}개의 이벤트가 발생할 예정이야! 주사위를 굴려보자!";
        }

        public void InitialDiceEndDescription()
        {
            initialDiceSetDialog.text =
                "\n모든 주사위 세팅이 완료됐어! 공연 시작할게~";
        }
    }
}