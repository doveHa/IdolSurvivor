using System.Data;
using System.Resources;
using Script;
using Script.TeamBuilding;
using TMPro;
using UnityEngine;
using ResourceManager = Script.Manager.ResourceManager;

namespace UI
{
    public class TeamBuildingDialogHandler : MonoBehaviour
    {
        private TextMeshProUGUI titleText;

        void Awake()
        {
            titleText = GetComponent<TextMeshProUGUI>();
        }

        void Start()
        {
            string text = string.Empty;
            Debug.Log(Config.Resource.StageData.CurrentStage);
            switch (Config.Resource.StageData.CurrentStage)
            {
                case Constant.Stage.STAGE_ONE:
                    text = "1차 공연 전 팀빌딩";
                    break;
                case Constant.Stage.STAGE_TWO:
                    text = "2차 공연 전 팀빌딩";
                    break;
                case Constant.Stage.FINAL_STAGE:
                    text = "최종 공연 전 팀빌딩";
                    break;
            }

            titleText.text = text;
        }
    }
}