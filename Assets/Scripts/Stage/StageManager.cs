using Script.DataDefinition.ScriptableObjects;
using Script.Manager;
using Script.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Stage
{
    public class StageManager : ManagerBase<StageManager>
    {
        public StageData CurrentStage { get; private set; }
        [SerializeField] private SpriteRenderer backGround;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Slider progressBar;

        [SerializeField] private RectTransform markerContainer;
        [SerializeField] private GameObject markerPrefab;
        private int progressTime = Constant.Stage.PROGRESS_TIME;
        private float currentTime = 0;

        private float[] eventTimes;
        private int currentEvent = 0;
        private bool isStop;

        protected override void Awake()
        {
            base.Awake();
            TitleTest();
            /*
            isStop = true;
            currentStage = ResourceManager.Load<StageData>(Config.Resource.StageData.NextStageDataPath());
            eventTimes = new float[currentStage.eventCount];
            */
        }

        void Start()
        {
            backGround.sprite = CurrentStage.backGround;
            title.text = CurrentStage.title;
            CreateMarker();
        }

        void Update()
        {
            if (!isStop)
            {
                AdjustProgressBar();
            }

            if (currentTime >= progressTime)
            {
                End();
            }
        }

        private void AdjustProgressBar()
        {
            currentTime += Time.deltaTime;
            float ratio = currentTime / progressTime;
            progressBar.value = ratio;

            if (currentTime >= eventTimes[currentEvent])
            {
                Pause();
                Debug.Log($"Event 발생! {currentTime}/{eventTimes[currentEvent]}");
                currentEvent++;
            }
        }

        private void CreateMarker()
        {
            float width = markerContainer.rect.width;

            float leftEdge = -width * markerContainer.pivot.x;
            for (int i = 0; i < CurrentStage.eventCount; i++)
            {
                float ratio = (float)(i + 1) / (CurrentStage.eventCount + 1);
                float xPos = leftEdge + width * ratio;

                GameObject marker = Instantiate(markerPrefab, markerContainer);
                marker.name = $"Marker_{ratio * 100f:F1}%";

                RectTransform rect = marker.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(xPos, 0);

                eventTimes[currentEvent] = progressTime * ratio;
                Debug.Log("이벤트 등록 " + eventTimes[currentEvent]);
                currentEvent++;
            }

            currentEvent = 0;
        }

        public void StageStart()
        {
            isStop = false;
        }

        private void Pause()
        {
            isStop = true;
        }

        private void End()
        {
            Pause();
            progressBar.GetComponentInChildren<UIAnimationHandler>().EndAnimation();
        }

        private void TitleTest()
        {
            isStop = true;
            Config.Resource.StageData.NextStage = Constant.Stage.TITLE_STAGE;
            CurrentStage = ResourceManager.Load<StageData>(Config.Resource.StageData.NextStageDataPath());
            CurrentStage.eventCount = 4;
            eventTimes = new float[CurrentStage.eventCount];
        }
    }
}