using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class AnnouncementManager : MonoBehaviour
{
    public static AnnouncementManager Instance { get; private set; }

    [Header("Data")]
    public List<TraineeData> allTrainees; // 모든 연습생 데이터 (순위 확정된 상태)
    public int currentRound = 1;         // 현재 몇 번째 결과 발표인지 (1, 2, 또는 3)
    private TraineeData playerTrainee;
    private bool isPlayerEliminated = false;

    [Header("UI Panels")]
    public GameObject rankingPanelGroup;   // 랭킹 패널 전체 (VerticalLayoutGroup 포함)
    public GameObject rankingEntryPrefab;  // 랭킹 패널에 들어갈 1인용 항목 프리팹
    public Transform rankingListParent;    // VerticalLayoutGroup이 적용된 부모 Transform
    public GameObject characterPanel;
    public GameObject divisionPanel;
    public GameObject gameOverPanel;

    [Header("Character Panel Details")]
    public TMPro.TextMeshProUGUI charRankText;
    public TMPro.TextMeshProUGUI charRankText2;
    public TMPro.TextMeshProUGUI charAgencyText;
    public TMPro.TextMeshProUGUI charNameText;
    public TMPro.TextMeshProUGUI charVotesText;
    public Image charImage;

    [Header("Division Panel Details")]
    public Image[] divisionImages = new Image[4];

    private int startingRankIndex;

    private int playerFinalRank = -1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // 초기화
        characterPanel.SetActive(false);
        divisionPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        // 순위 정렬
        allTrainees = allTrainees.OrderBy(t => t.rank).ToList();
        playerTrainee = allTrainees.FirstOrDefault(t => t.isPlayer);

        SetStartingRank();
    }

    private void SetStartingRank()
    {
        // 순위는 1부터 시작합니다.
        if (currentRound == 1)
            startingRankIndex = 9; // 9위부터 발표 시작
        else if (currentRound == 2)
            startingRankIndex = 6; // 6위부터 발표 시작 (9, 8, 7위는 탈락자로 간주)
        else if (currentRound == 3)
            startingRankIndex = 2; // 6?? 3?? 2??

        // 랭킹 패널 UI 초기화
        foreach (Transform child in rankingListParent)
            Destroy(child.gameObject);
    }

    void Start()
    {
        StartCoroutine(StartResultsAnnouncement());
    }

    /// <summary>
    /// 전체 결과 발표 시퀀스를 시작합니다.
    /// </summary>
    public IEnumerator StartResultsAnnouncement()
    {
        // 씬 시작 대사
        yield return StartDialogueCoroutine(new string[] { "지금부터 생존할 연습생을 발표합니다." });

        if (currentRound == 3)
        {
            yield return StartCoroutine(AnnounceFinalRound());
        }
        else
        {
            yield return StartCoroutine(AnnounceNormalRound());
        }

        // 최종 정리 (탈락 여부 확인)
        yield return StartCoroutine(FinalizeResults());
    }

    // -----------------------------------------------------------
    // 1, 2차 발표 (일반적인 순위 발표)
    // -----------------------------------------------------------

    private IEnumerator AnnounceNormalRound()
    {
        int totalToAnnounce = (currentRound == 1) ? 9 : 6;
        int currentRank = startingRankIndex; // 9위 또는 6위부터 시작

        // 9위 (또는 6위)부터 1위까지 순차적으로 발표
        while (currentRank >= 1 && currentRank <= totalToAnnounce)
        {
            TraineeData trainee = allTrainees.Find(t => t.rank == currentRank);

            if (trainee.rank == 0) // 해당 순위에 연습생이 없는 경우 (범위를 벗어난 경우)
            {
                currentRank--;
                continue;
            }

            // 랭킹 패널 업데이트 (좌측 이름 목록 출력)
            UpdateRankingList(trainee, currentRank);

            // 플레이어 확인
            if (trainee.isPlayer)
            {
                playerFinalRank = currentRank;
                yield return StartCoroutine(PlayerAnnounceSequence(trainee));
                // 플레이어 발표 후 RankingPanel 비활성화, CharacterPanel 활성화 상태 유지

                // 플레이어 발표 후, 다음 순위로 이동
                currentRank--;
                continue;
            }

            // 일반 연습생 순위 발표
            yield return StartDialogueCoroutine(new string[] { $"다음은 {currentRank}위 입니다." });
            yield return new WaitForSeconds(1.0f); // 긴장감 유지

            currentRank--;
        }
    }

    private void UpdateRankingList(TraineeData trainee, int rank)
    {
        // 랭킹 목록에 해당 순위와 이름만 표시 (VerticalLayoutGroup)
        GameObject newEntry = Instantiate(rankingEntryPrefab, rankingListParent);
        // TMP 컴포넌트를 찾아 순위와 이름 할당 (프리팹 구조에 맞게 수정 필요)
        newEntry.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{rank}위: {trainee.traineeName}";
    }

    // -----------------------------------------------------------
    // 플레이어 발표 시퀀스
    // -----------------------------------------------------------

    private IEnumerator PlayerAnnounceSequence(TraineeData player)
    {
        // RankingPanel (왼쪽 목록) 비활성화
        rankingPanelGroup.SetActive(false);

        // CharacterPanel 활성화 및 데이터 할당
        characterPanel.SetActive(true);
        charRankText.text = player.rank.ToString();
        charNameText.text = player.traineeName;
        charAgencyText.text = player.agency;
        charVotesText.text = player.votes.ToString("N0") + "표";
        charImage.sprite = player.characterImage;

        yield return StartDialogueCoroutine(new string[]
            {
                $"모두가 주목하고 있습니다! {player.traineeName} 연습생!",
                $"{player.traineeName} 연습생이 최종 {player.rank}위로 생존합니다!"
            });

        yield return new WaitForSeconds(2.0f);
    }

    // -----------------------------------------------------------
    // 3차 발표 (Division Panel 사용)
    // -----------------------------------------------------------

    private IEnumerator AnnounceFinalRound()
    {
        // 1. 2위 발표
        TraineeData secondPlace = allTrainees.Find(t => t.rank == 2);
        yield return StartDialogueCoroutine(new string[] { "이제 최종 데뷔조, 2위를 발표합니다." });
        UpdateRankingList(secondPlace, 2);
        yield return new WaitForSeconds(1.5f);

        // 2. 1위 발표 (플레이어일 수도 있음)
        TraineeData firstPlace = allTrainees.Find(t => t.rank == 1);
        if (firstPlace.isPlayer)
        {
            playerFinalRank = 1;
            yield return StartCoroutine(PlayerAnnounceSequence(firstPlace));
        }
        else
        {
            yield return StartDialogueCoroutine(new string[] { "대망의 1위! 과연 누가 될까요?" });
            UpdateRankingList(firstPlace, 1);
            yield return new WaitForSeconds(1.5f);
        }

        // 3. Division Panel 활성화 및 6위~3위 이미지 할당
        divisionPanel.SetActive(true);
        yield return StartDialogueCoroutine(new string[] { "남은 6위부터 3위까지의 연습생을 공개합니다!" });

        var divisionTrainees = allTrainees.Where(t => t.rank >= 3 && t.rank <= 6)
                                          .OrderByDescending(t => t.rank) // 6위부터 순차적으로 처리
                                          .ToList();

        for (int i = 0; i < divisionTrainees.Count; i++)
        {
            if (i < divisionImages.Length)
            {
                divisionImages[i].sprite = divisionTrainees[i].characterImage;
                divisionImages[i].gameObject.SetActive(true);
            }
        }

        // 4. 3위 발표 (Division Panel에 이미지 할당 후 3위 발표)
        TraineeData thirdPlace = allTrainees.Find(t => t.rank == 3);
        yield return StartDialogueCoroutine(new string[] { $"최종 데뷔조, 마지막 멤버 {thirdPlace.traineeName} 연습생!" });
        UpdateRankingList(thirdPlace, 3);

        yield return new WaitForSeconds(2.0f);
    }

    // -----------------------------------------------------------
    // 최종 마무리 (탈락 처리)
    // -----------------------------------------------------------

    private IEnumerator FinalizeResults()
    {
        // 플레이어가 순위에 없고 (탈락했고), 탈락 발표가 필요한 경우
        if (playerTrainee.rank > startingRankIndex || playerFinalRank == -1)
        {
            yield return StartDialogueCoroutine(new string[]
                {
                    "아쉽게도 이번 발표에서는 이름을 올리지 못했습니다.",
                    $"{playerTrainee.traineeName} 연습생은 아쉽게 탈락했습니다..."
                });

            gameOverPanel.SetActive(true); // 게임 오버 패널 활성화
        }
        else
        {
            yield return StartDialogueCoroutine(new string[] { "발표를 모두 마쳤습니다. 감사합니다." });
        }
    }

    // -----------------------------------------------------------
    // GMManager 연동 도우미 (GMManager가 DontDestroyOnLoad 되어 있어야 함)
    // -----------------------------------------------------------

    private IEnumerator StartDialogueCoroutine(string[] dialogue)
    {
        if (GMManager.Instance == null)
        {
            Debug.LogError("GMManager 인스턴스를 찾을 수 없습니다.");
            yield break;
        }

        bool waitingForDialogue = true;

        // GMManager의 콜백을 설정하여 대화 종료 시 waitingForDialogue를 false로 변경
        GMManager.Instance.StartDialogue(dialogue, () => waitingForDialogue = false);

        // 대화가 끝날 때까지 대기
        while (waitingForDialogue)
        {
            yield return null;
        }
    }
}
