using TMPro;
using UnityEngine;

namespace Script.Stage
{
    public class StageDialogManager : MonoBehaviour
    {
        public static StageDialogManager Manager { get; private set; }
        [SerializeField] private TextMeshProUGUI initialDiceSetDialog;
        [SerializeField] private TextMeshProUGUI titleDialog;

        void Awake()
        {
            if (Manager == null)
            {
                Manager = this;
            }
        }

        void Start()
        {
            DialogInitialize();
        }

        private void DialogInitialize()
        {
            InitialDiceSettingDescription();
            InitialTitleDescription();
        }

        private void InitialDiceSettingDescription()
        {
            initialDiceSetDialog.text =
                $"\n 총 {Config.Event.EventCount}개의 이벤트가 발생할 예정이야! 주사위를 굴려보자!";
        }

        private void InitialTitleDescription()
        {
            Debug.Log(StageManager.Manager.CurrentStage.title);
            titleDialog.text = StageManager.Manager.CurrentStage.title;
        }

        public void InitialDiceEndDescription()
        {
            initialDiceSetDialog.text =
                "\n모든 주사위 세팅이 완료됐어! 공연 시작할게~";
        }
    }
}