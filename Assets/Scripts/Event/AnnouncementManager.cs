using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script;
using Script.Characters;
using Script.Manager;
using TMPro;
using UnityEngine.SceneManagement;

public class AnnouncementManager : MonoBehaviour
{
    public static AnnouncementManager Instance { get; private set; }

    [Header("Data")] public List<TraineeData> allTrainees; // 모든 훈련생 데이터(순위 확인용)
    private TraineeData? playerTrainee; // 값 형식(struct)이라면 Nullable 타입으로 변경
    private bool isPlayerEliminated = false;

    [Header("UI Panels")] public GameObject rankingPanelGroup; // 랭킹 패널 전체 (VerticalLayoutGroup 포함)
    public GameObject rankingEntryPrefab; /// 랭킹 패널에 들어갈 1인용 항목 프리팹
    public Transform rankingListParent; // VerticalLayoutGroup parent Transform
    public Image Ranking_charImage;
    public TMPro.TextMeshProUGUI Ranking_charNameText;
    public TextMeshProUGUI Ranking_charRankText;
    public GameObject NextBtn;
    public GameObject characterPanel;
    public GameObject divisionPanel;
    public GameObject gameOverPanel;

    [Header("Character Panel Details")] public TMPro.TextMeshProUGUI charRankText;
    public TMPro.TextMeshProUGUI charRankText2;
    public TMPro.TextMeshProUGUI charAgencyText;
    public TMPro.TextMeshProUGUI charNameText;
    public TMPro.TextMeshProUGUI charVotesText;
    public Image charImage;

    [Header("Division Panel Details")] public Image[] divisionImages = new Image[4];

    private int startingRankIndex;

    private int playerFinalRank = -1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        characterPanel.SetActive(false);
        divisionPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        rankingPanelGroup.SetActive(true);

        // 데이터 설정
        SetAllTrainees();

        // allTrainees에서 플레이어 트레이니 데이터를 찾음
        playerTrainee = allTrainees.FirstOrDefault(t => t.isPlayer);
        if (playerTrainee is null) // Nullable 타입은 is null 체크 가능
        {
            // 플레이어 트레이니 데이터가 없는 경우를 대비 (오류 방지)
            Debug.Log($"Player Trainee Loaded: {playerTrainee.Value.traineeName}, Rank: {playerTrainee.Value.rank}");
        }

        SetStartingRank();
    }

    private void SetAllTrainees()
    {
        allTrainees = new List<TraineeData>();

        // AllCharacterManager.Manager.Ranks에서 데이터 로드
        foreach (Character character in AllCharacterManager.Manager.Ranks)
        {
            allTrainees.Add(new TraineeData(character));
        }

        // 순위 기준으로 정렬
        allTrainees = allTrainees.OrderBy(t => t.rank).ToList();
    }

    private void SetStartingRank()
    {
        Debug.Log(Config.Resource.StageData.CurrentStage);

        // 최저 순위부터 발표하기 위해 순위 인덱스를 설정합니다.
        switch (Config.Resource.StageData.CurrentStage)
        {
            case Constant.Stage.STAGE_ONE:
                startingRankIndex = 9; // 9등부터 발표 시작
                break;
            case Constant.Stage.STAGE_TWO:
                startingRankIndex = 6; // 6등부터 발표 시작
                break;
            case Constant.Stage.FINAL_STAGE:
                startingRankIndex = 3; // 3위부터 발표 시작 (1, 2위를 마지막에 발표)
                break;
        }

        // 랭킹 패널 UI 초기화
        foreach (Transform child in rankingListParent)
            Destroy(child.gameObject);
    }

    void Start()
    {
        StartCoroutine(StartResultsAnnouncement());
    }

    /// <summary>
    /// 전체 결과 발표 순서를 관리합니다.
    /// </summary>
    public IEnumerator StartResultsAnnouncement()
    {
        // Debug Log for testing
        Debug.Log($"<color=yellow>--- {Config.Resource.StageData.CurrentStage} 결과 발표 시작 ---</color>");
        Debug.Log($"발표 시작 순위: {startingRankIndex}등부터");
        if (playerTrainee is not null)
        {
            Debug.Log($"플레이어 현재 순위: {playerTrainee.Value.rank}등 (발표 대상 기준: {startingRankIndex}등 이내)");
        }
        else
        {
            Debug.LogError("Player Trainee Data is null! (Did not find player in ranks)");
        }

        Debug.Log("--- 전체 연습생 순위 (정렬 기준) ---");
        foreach (var trainee in allTrainees)
        {
            Debug.Log($"Rank: {trainee.rank}, Name: {trainee.traineeName}, Votes: {trainee.votes}");
        }
        Debug.Log("-------------------------------------");


        // 오프닝 멘트
        yield return StartDialogueCoroutine(new string[] { "지금부터 순위를 발표하겠습니다." });

        if (Config.Resource.StageData.CurrentStage.Equals(Constant.Stage.FINAL_STAGE))
        {
            yield return StartCoroutine(AnnounceFinalRound());
        }
        else
        {
            yield return StartCoroutine(AnnounceNormalRound());
            // 탈락자 처리 로직
            AllCharacterManager.Manager.DropOut();
        }


        // 발표 마무리 (탈락 여부 확인)
        yield return StartCoroutine(FinalizeResults());

        characterPanel.SetActive(false);
        divisionPanel.SetActive(false);
        rankingPanelGroup.SetActive(false);

        // FinalizeResults에서 게임 오버가 아닐 경우 다음 스테이지 버튼 활성화 또는 자동 진행
        if (gameOverPanel.activeSelf == false)
        {
            // 다음 스테이지로 진행하는 로직 (예: 버튼 노출, 자동 씬 전환)
            Debug.Log("순위권 진입 성공! 다음 스테이지로!");
            yield return new WaitForSeconds(3.0f);
            // OnNextStage() 호출은 버튼 클릭 이벤트로 연결되어 있다고 가정
        }
    }

    // -----------------------------------------------------------
    // 1, 2차 발표 (일반 라운드 순위 발표)
    // -----------------------------------------------------------

    private IEnumerator AnnounceNormalRound()
    {
        rankingPanelGroup.SetActive(true);
        characterPanel.SetActive(false);
        divisionPanel.SetActive(false);

        int totalToAnnounce = Config.Resource.StageData.CurrentStage.Equals(Constant.Stage.STAGE_ONE) ? 9 : 6;
        int currentRank = startingRankIndex; // 9등 또는 6등부터 시작

        // 최저 순위부터 1등까지 거꾸로 발표
        while (currentRank >= 1 && currentRank <= totalToAnnounce)
        {
            // **[수정 3] 해당 순위의 모든 연습생(공동 순위)을 가져옴**
            var traineesToAnnounce = allTrainees.Where(t => t.rank == currentRank).ToList();

            if (!traineesToAnnounce.Any())
            {
                Debug.LogWarning($"Rank {currentRank} Trainee not found in allTrainees. Skipping.");
                currentRank--;
                continue;
            }

            // 해당 순위의 모든 연습생에 대해 발표 진행
            foreach (var trainee in traineesToAnnounce)
            {
                // 버그 발생 임시 방어
                if (trainee.rank == 0)
                {
                    Debug.LogWarning($"Skipping Trainee {trainee.traineeName} due to invalid rank (0).");
                    continue;
                }

                // 랭킹 리스트 업데이트 (이름 표시)
                UpdateRankingList(trainee, currentRank);

                UpdateRankingMainUI(trainee, currentRank);

                // Player ranking
                if (trainee.isPlayer)
                {
                    playerFinalRank = currentRank;
                    yield return StartCoroutine(PlayerAnnounceSequence(trainee));

                    // 플레이어 발표 후, CharacterPanel 비활성화 및 RankingPanelGroup 재활성화
                    characterPanel.SetActive(false);
                    rankingPanelGroup.SetActive(true);
                }
                else
                {
                    // 일반 연습생 순위 발표
                    Debug.Log($"<color=cyan>발표: {currentRank}위 {trainee.traineeName}</color>");
                    yield return StartDialogueCoroutine(new string[] { $"다음은 {currentRank}위 입니다." });
                    yield return new WaitForSeconds(1.0f); // 긴장감 조성
                    yield return StartDialogueCoroutine(new string[] { $"축하합니다! {trainee.traineeName} 연습생 입니다." });
                    yield return new WaitForSeconds(1.0f);
                }
            }

            // 공동 순위 발표가 완료되면 다음 순위로 이동
            currentRank--;
        }
    }

    private void UpdateRankingMainUI(TraineeData trainee, int rank)
    {
        // 랭킹 메인 UI (이미지, 이름, 순위) 업데이트
        if (Ranking_charImage is not null)
        {
            Ranking_charImage.sprite = trainee.characterImage;
            Ranking_charImage.gameObject.SetActive(true); // 혹시 비활성화되어 있다면 활성화
        }
        if (Ranking_charNameText is not null)
        {
            Ranking_charNameText.text = trainee.traineeName;
        }
        if (Ranking_charRankText is not null)
        {
            Ranking_charRankText.text = $"{rank}위";
        }
    }


    private void UpdateRankingList(TraineeData trainee, int rank)
    {
        if (rankingEntryPrefab == null)
        {
            Debug.LogError("Ranking Entry Prefab is not assigned. Check the Inspector!");
            return;
        }

        GameObject newEntry = Instantiate(rankingEntryPrefab, rankingListParent);

        if (!newEntry.activeSelf)
        {
            newEntry.SetActive(true);
        }

        // TMP 컴포넌트를 찾아 순위와 이름 할당
        TMPro.TextMeshProUGUI tmpText = newEntry.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmpText is not null)
        {
            string name = string.IsNullOrEmpty(trainee.traineeName) ? "(이름 없음)" : trainee.traineeName;
            tmpText.text = $"{rank}위: {trainee.traineeName}";
        }
        else
        {
            Debug.LogError("Ranking Entry Prefab is missing TextMeshProUGUI component in children.");
        }
        // UI가 잘 작동되는지 확인하기 위한 디버그
        Debug.Log($"UI 업데이트: 랭킹 리스트에 {rank}위 {trainee.traineeName} 추가");
    }

    // -----------------------------------------------------------
    // 플레이어 발표 시퀀스
    // -----------------------------------------------------------

    private IEnumerator PlayerAnnounceSequence(TraineeData player)
    {
        // RankingPanel (순위 목록) 비활성화
        rankingPanelGroup.SetActive(false);

        // CharacterPanel 활성화 및 데이터 할당
        characterPanel.SetActive(true);
        // TraineeData가 값 형식이라도 매개변수로 넘어왔다면 null이 아님.
        charRankText.text = player.rank.ToString();
        charNameText.text = player.traineeName;
        charAgencyText.text = player.agency;
        charVotesText.text = player.votes.ToString("N0") + "표";
        charImage.sprite = player.characterImage;

        Debug.Log($"플레이어 발표: {player.rank}위 {player.traineeName}");

        yield return StartDialogueCoroutine(new string[]
        {
            $"모두가 주목하고 있습니다! {player.traineeName} 연습생!",
            $"{player.traineeName} 연습생은 최종 {player.rank}위로 생존/데뷔에 성공했습니다!"
        });

        yield return new WaitForSeconds(2.0f);
    }

    // -----------------------------------------------------------
    // 최종 발표 (Division Panel 포함)
    // -----------------------------------------------------------

    private IEnumerator AnnounceFinalRound()
    {
        // 1. 2위 발표
        TraineeData? secondPlace = allTrainees.FirstOrDefault(t => t.rank == 2); // ⬅️ Nullable 타입으로 변경
        if (secondPlace is not null)
        {
            yield return StartDialogueCoroutine(new string[] { "마지막 데뷔 멤버, 2위를 발표합니다." });
            // 2위 발표 순위 UI 업데이트
            UpdateRankingList(secondPlace.Value, 2); // .Value 사용
            if (secondPlace.Value.isPlayer)
            {
                playerFinalRank = 2;
                yield return StartCoroutine(PlayerAnnounceSequence(secondPlace.Value)); // .Value 사용
                characterPanel.SetActive(false);
                rankingPanelGroup.SetActive(true);
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
                yield return StartDialogueCoroutine(new string[] { $"축하합니다! 2위는 {secondPlace.Value.traineeName} 연습생입니다!" });
                yield return new WaitForSeconds(1.5f);
            }
        }

        // 2. 1위 발표
        TraineeData? firstPlace = allTrainees.FirstOrDefault(t => t.rank == 1); // ⬅️ Nullable 타입으로 변경
        if (firstPlace is not null)
        {
            yield return StartDialogueCoroutine(new string[] { "영광의 1위! 과연 센터는 누구일까요?" });
            // 1위 발표 순위 UI 업데이트
            UpdateRankingList(firstPlace.Value, 1); // .Value 사용
            if (firstPlace.Value.isPlayer)
            {
                playerFinalRank = 1;
                yield return StartCoroutine(PlayerAnnounceSequence(firstPlace.Value)); // .Value 사용
                characterPanel.SetActive(false);
                rankingPanelGroup.SetActive(true);
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
                yield return StartDialogueCoroutine(new string[] { $"축하합니다! 최종 1위는 {firstPlace.Value.traineeName} 연습생입니다!" });
                yield return new WaitForSeconds(1.5f);
            }
        }

        // 3. Division Panel 활성화 및 6위~3위 후보 이미지 할당
        divisionPanel.SetActive(true);
        rankingPanelGroup.SetActive(false); // 랭킹 리스트 숨김
        characterPanel.SetActive(false); // 캐릭터 패널 숨김

        yield return StartDialogueCoroutine(new string[] { "다음은 아쉽게 데뷔에 실패한 6위부터 3위 후보를 공개합니다!" });

        // 6위, 5위, 4위, 3위 순으로 이미지 설정 (OrderByDescending(t => t.rank)로 6->5->4->3 순)
        // Linq의 Where, OrderByDescending은 Nullable을 허용하지 않으므로, 이 리스트는 TraineeData로 유지
        var divisionTrainees = allTrainees.Where(t => t.rank >= 3 && t.rank <= 6)
            .OrderByDescending(t => t.rank)
            .ToList();

        // Division Panel 이미지 초기화 및 할당
        for (int i = 0; i < divisionImages.Length; i++)
        {
            divisionImages[i].gameObject.SetActive(false); // 일단 모두 비활성화
        }

        for (int i = 0; i < divisionTrainees.Count; i++)
        {
            if (i < divisionImages.Length)
            {
                divisionImages[i].sprite = divisionTrainees[i].characterImage;
                divisionImages[i].gameObject.SetActive(true);
                Debug.Log($"<color=yellow>Division Panel: {divisionTrainees[i].rank}위 {divisionTrainees[i].traineeName} 이미지 할당</color>");
            }
        }

        yield return new WaitForSeconds(3.0f); // 후보들을 충분히 보여주는 시간

        // 4. 3위 발표
        TraineeData? thirdPlace = allTrainees.FirstOrDefault(t => t.rank == 3); // ⬅️ Nullable 타입으로 변경
        if (thirdPlace is not null)
        {
            yield return StartDialogueCoroutine(
                new string[] { $"최종 데뷔의 마지막 자리, 3위를 차지한 주인공은... {thirdPlace.Value.traineeName} 연습생 입니다!" });
            UpdateRankingList(thirdPlace.Value, 3); // .Value 사용

            // 3위가 플레이어인 경우
            if (thirdPlace.Value.isPlayer)
            {
                divisionPanel.SetActive(false); // 디비전 패널 숨기고
                playerFinalRank = 3;
                yield return StartCoroutine(PlayerAnnounceSequence(thirdPlace.Value)); // .Value 사용
                rankingPanelGroup.SetActive(true); // 다음 로직을 위해 다시 활성화
            }
            else
            {
                // 3위 강조 연출 (예: 3위 이미지 외 나머지 어둡게 처리 등) 
                yield return new WaitForSeconds(2.0f);
            }
        }
        else
        {
            Debug.LogError("3rd Place Trainee not found!");
        }

        divisionPanel.SetActive(false);
    }

    // -----------------------------------------------------------
    // 최종 마무리 (탈락 처리)
    // -----------------------------------------------------------

    private IEnumerator FinalizeResults()
    {
        // Nullable 타입은 .Value를 사용해야 rank에 접근 가능하며, is not null 체크가 선행되어야 안전함
        bool isPlayerEliminated = (playerTrainee is not null && playerTrainee.Value.rank > startingRankIndex) || playerFinalRank == -1;

        if (isPlayerEliminated)
        {
            // Nullable 타입에 안전하게 접근하기 위해 삼항 연산자 사용 (is null 체크)
            string playerRankStr = playerTrainee is null ? "N/A" : playerTrainee.Value.rank.ToString();
            string playerNameStr = playerTrainee is null ? "연습생" : playerTrainee.Value.traineeName;

            Debug.Log($"<color=red>플레이어 탈락 처리: 최종 순위 {playerRankStr} (탈락 기준: {startingRankIndex}위 이상)</color>");

            // 모든 UI 패널 숨김 (필요에 따라)
            characterPanel.SetActive(false);
            divisionPanel.SetActive(false);
            rankingPanelGroup.SetActive(false);

            yield return StartDialogueCoroutine(new string[]
            {
                "아쉽지만 이번 순위 발표에서 데뷔/생존권에 들지 못했습니다.",
                $"{ playerNameStr } 연습생은 아쉽게도 탈락했습니다..." // 수정된 playerNameStr 사용
            });

            gameOverPanel.SetActive(true); // 게임 오버 패널 활성화 (요청하신 순서)
            NextBtn.SetActive(false); // 다음 스테이지 버튼 비활성화
        }
        else
        {
            Debug.Log($"<color=green>플레이어 생존/데뷔 성공: 최종 발표 순위 {playerFinalRank}위</color>");
            yield return StartDialogueCoroutine(new string[] { "발표는 모두 마쳤습니다. 축하드립니다." });
        }
    }

    // -----------------------------------------------------------
    // GMManager 관련 (GMManager가 DontDestroyOnLoad 되어 있어야 함)
    // -----------------------------------------------------------

    private IEnumerator StartDialogueCoroutine(string[] dialogue)
    {
        if (GMManager.Instance is null)
        {
            Debug.LogError("GMManager 인스턴스를 찾을 수 없습니다.");
            yield break;
        }

        bool waitingForDialogue = true;

        // GMManager에 콜백을 넘겨 대화 종료 시 waitingForDialogue를 false로 설정
        GMManager.Instance.StartDialogue(dialogue, () => waitingForDialogue = false);

        // 대화가 끝날 때까지 대기
        while (waitingForDialogue)
        {
            yield return null;
        }
    }

    public void OnNextStage()
    {
        if (PracticeManager.Instance == null)
        {
            Debug.LogError("PracticeManager instance not found! Cannot set target stage constant.");
        }

        switch (Config.Resource.StageData.CurrentStage)
        {
            case Constant.Stage.STAGE_ONE:
                Config.Team.TeamCount = 3; // 4 -> 3
                Config.Team.AllCharacterCount = 9; // 12 -> 9
                Debug.Log(Config.Team.TeamCount);
                Debug.Log(Config.Team.AllCharacterCount);
                Config.Resource.StageData.CurrentStage = Constant.Stage.STAGE_TWO;
                SceneManager.LoadScene("MiniEvent1");
                break;
            case Constant.Stage.STAGE_TWO:
                Config.Team.TeamCount = 2;
                Config.Team.AllCharacterCount = 6;
                Debug.Log(Config.Team.TeamCount);
                Debug.Log(Config.Team.AllCharacterCount);
                Config.Resource.StageData.CurrentStage = Constant.Stage.FINAL_STAGE;
                SceneManager.LoadScene("MiniEvent2");
                break;
            case Constant.Stage.FINAL_STAGE:
                Debug.Log("Congratulations!");
                break;
        }
    }
}