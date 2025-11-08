using Script.DataDefinition.ScriptableObjects;
using Script.Manager;
using Script.Stage.Event;
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
            isStop = true;
            CurrentStage = ResourceManager.Load<StageData>(Config.Resource.StageData.CurrentStageDataPath());
            eventTimes = new float[EventConfig.EventCount];
            EventConfig.EventSetting();
        }

        void Start()
        {
            backGround.sprite = CurrentStage.backGround;
            title.text = CurrentStage.title;
            CreateMarker();
        }

        void Update()
        {
            if (currentTime >= progressTime)
            {
                End();
            }

            if (!isStop)
            {
                AdjustProgressBar();
            }
        }

        private void AdjustProgressBar()
        {
            currentTime += Time.deltaTime;
            float ratio = currentTime / progressTime;
            progressBar.value = ratio;

            if (currentEvent < eventTimes.Length && currentTime >= eventTimes[currentEvent])
            {
                TimePause();
                Debug.Log($"Event 발생! {currentTime}/{eventTimes[currentEvent]}");
                StageFlowManager.Manager.EventOccured();
                currentEvent++;
            }
        }

        private void CreateMarker()
        {
            float width = markerContainer.rect.width;

            float leftEdge = -width * markerContainer.pivot.x;
            for (int i = 0; i < EventConfig.EventCount; i++)
            {
                float ratio = (float)(i + 1) / (EventConfig.EventCount + 1);
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

        public void TimeStart()
        {
            progressBar.GetComponentInChildren<UIAnimationHandler>().StartAnimation();
            isStop = false;
        }

        private void TimePause()
        {
            progressBar.GetComponentInChildren<UIAnimationHandler>().PauseAnimation();
            isStop = true;
        }

        private void End()
        {
            progressBar.GetComponentInChildren<UIAnimationHandler>().EndAnimation();
            // 결과 화면으로 Scene 전환
        }
    }
}