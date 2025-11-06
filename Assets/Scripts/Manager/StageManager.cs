using Script.DataDefinition.Enum;
using Script.DataDefinition.ScriptableObjects;
using TMPro;
using UnityEngine;

namespace Script.Manager
{
    public class StageManager : ManagerBase<StageManager>
    {
        private StageData currentStage;
        [SerializeField] private SpriteRenderer backGround;
        [SerializeField] private TextMeshProUGUI title;

        private int progressTime = Constant.Stage.PROGRESS_TIME;

        protected override void Awake()
        {
            base.Awake();
            currentStage = ResourceManager.Load<StageData>(Config.Resource.StageData.NextStageDataPath());
        }

        void Start()
        {
            backGround.sprite = currentStage.backGround;
            title.text = currentStage.title;
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}