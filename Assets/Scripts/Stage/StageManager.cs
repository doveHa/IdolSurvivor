using Script.Characters;
using Script.DataDefinition.Data;
using Script.DataDefinition.Enum;
using Script.DataDefinition.ScriptableObjects;
using Script.Manager;
using Script.TeamBuilding;
using Script.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Stage
{
    public class StageManager : ManagerBase<StageManager>
    {
        public static AudioClip BGMClip { get; private set; }

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
            eventTimes = new float[Config.Event.EventCount];
            Config.Event.EventSetting();
        }

        void Start()
        {
            backGround.sprite = CurrentStage.backGround;
            title.text = CurrentStage.title;
            CreateMarker();

            // BGM 재생
            gameObject.GetComponent<AudioSource>().clip = StageManager.BGMClip;
            gameObject.GetComponent<AudioSource>().Play();
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
            for (int i = 0; i < Config.Event.EventCount; i++)
            {
                float ratio = (float)(i + 1) / (Config.Event.EventCount + 1);
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

        public void SetPlusStat(StatType plusType)
        {
            CurrentStage.plusStat = plusType;
        }


        public void SetMinusStat(StatType minusType)
        {
            CurrentStage.minusStat = minusType;
        }

        private void TimePause()
        {
            progressBar.GetComponentInChildren<UIAnimationHandler>().PauseAnimation();
            isStop = true;
        }

        private void End()
        {
            progressBar.GetComponentInChildren<UIAnimationHandler>().EndAnimation();
            AdjustOtherCharacterResult();

            AllCharacterManager.Manager.CalculateRank();
            SceneLoadManager.StageEndScene();
            Destroy(gameObject);
            // 결과 화면으로 Scene 전환
        }

        private void AdjustPlayerCharacterResult()
        {
        }

        private void AdjustOtherCharacterResult()
        {
            if (Config.Resource.StageData.CurrentStage.Equals(Constant.Stage.TITLE_STAGE))
            {
                foreach (Character character in AllCharacterManager.Manager.OtherCharacters)
                {
                    character.AddVote(Random.Range(2, 6));
                }
            }
            else
            {
                //투표 수 반영
            }
        }

        // BGM 설정
        public static void SetNextBGM(AudioClip bgm)
        {
            BGMClip = bgm;
        }
    }
}