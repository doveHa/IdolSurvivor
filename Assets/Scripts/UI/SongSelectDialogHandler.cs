using Script;
using TMPro;
using UnityEngine;

namespace UI
{
    public class SongSelectDialogHandler : MonoBehaviour
    {
        private TextMeshProUGUI titleText;

        void Awake()
        {
            titleText = GetComponent<TextMeshProUGUI>();
        }

        void Start()
        {
            string text = string.Empty;
            switch (Config.Resource.StageData.CurrentStage)
            {
                case Constant.Stage.TITLE_STAGE:
                    text = "1차 공연 공연곡 선택";
                    break;
                case Constant.Stage.STAGE_ONE:
                    text = "2차 공연 공연곡 선택";
                    break;
                case Constant.Stage.STAGE_TWO:
                    text = "최종 공연 공연곡 선택";
                    break;
                
            }

            titleText.text = text;
        }
    }
}