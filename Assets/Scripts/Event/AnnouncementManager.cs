using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Script;
using Script.Characters;
using Script.Manager;
using TMPro;

public class AnnouncementManager : MonoBehaviour
{
    public static AnnouncementManager Instance { get; private set; }

    [Header("Data")] public List<TraineeData> allTrainees; // ��� ������ ������ (���� Ȯ���� ����)
    private TraineeData playerTrainee;
    private bool isPlayerEliminated = false;

    [Header("UI Panels")] public GameObject rankingPanelGroup; // ��ŷ �г� ��ü (VerticalLayoutGroup ����)
    public GameObject rankingEntryPrefab; // ��ŷ �гο� �� 1�ο� �׸� ������
    public Transform rankingListParent; // VerticalLayoutGroup�� ����� �θ� Transform
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

        // �ʱ�ȭ
        characterPanel.SetActive(false);
        divisionPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        // ���� ����
        SetAllTrainees();
        playerTrainee = new TraineeData(AllCharacterManager.Manager.Player);
        playerTrainee.isPlayer = true;

        //allTrainees = allTrainees.OrderBy(t => t.rank).ToList();
        //playerTrainee = allTrainees.FirstOrDefault(t => t.isPlayer);

        SetStartingRank();
    }

    private void SetAllTrainees()
    {
        foreach (Character character in AllCharacterManager.Manager.Ranks)
        {
            allTrainees.Add(new TraineeData(character));
        }
    }

    private void SetStartingRank()
    {
        // ������ 1���� �����մϴ�.
        switch (Config.Resource.StageData.CurrentStage)
        {
            case Constant.Stage.STAGE_ONE:
                startingRankIndex = 9; // 9������ ��ǥ ����
                break;
            case Constant.Stage.STAGE_TWO:
                startingRankIndex = 6; // 6������ ��ǥ ���� (9, 8, 7���� Ż���ڷ� ����)
                break;
            case Constant.Stage.FINAL_STAGE:
                startingRankIndex = 2; // 6?? 3?? 2??
                break;
        }

        // ��ŷ �г� UI �ʱ�ȭ
        foreach (Transform child in rankingListParent)
            Destroy(child.gameObject);
    }

    void Start()
    {
        StartCoroutine(StartResultsAnnouncement());
    }

    /// <summary>
    /// ��ü ��� ��ǥ �������� �����մϴ�.
    /// </summary>
    public IEnumerator StartResultsAnnouncement()
    {
        // �� ���� ���
        yield return StartDialogueCoroutine(new string[] { "���ݺ��� ������ �������� ��ǥ�մϴ�." });

        if (Config.Resource.StageData.CurrentStage.Equals(Constant.Stage.FINAL_STAGE))
        {
            yield return StartCoroutine(AnnounceFinalRound());
        }
        else
        {
            yield return StartCoroutine(AnnounceNormalRound());
            AllCharacterManager.Manager.DropOut();
        }


        // ���� ���� (Ż�� ���� Ȯ��)
        yield return StartCoroutine(FinalizeResults());
    }

    // -----------------------------------------------------------
    // 1, 2�� ��ǥ (�Ϲ����� ���� ��ǥ)
    // -----------------------------------------------------------

    private IEnumerator AnnounceNormalRound()
    {
        int totalToAnnounce;
        if (Config.Resource.StageData.CurrentStage.Equals(Constant.Stage.STAGE_ONE))
        {
            totalToAnnounce = 9;
        }
        else
        {
            totalToAnnounce = 6;
        }

        int currentRank = startingRankIndex; // 9�� �Ǵ� 6������ ����

        // 9�� (�Ǵ� 6��)���� 1������ ���������� ��ǥ
        while (currentRank >= 1 && currentRank <= totalToAnnounce)
        {
            TraineeData trainee = allTrainees.Find(t => t.rank == currentRank);

            if (trainee.rank == 0) // �ش� ������ �������� ���� ��� (������ ��� ���)
            {
                currentRank--;
                continue;
            }

            // ��ŷ �г� ������Ʈ (���� �̸� ��� ���)
            UpdateRankingList(trainee, currentRank);

            // �÷��̾� Ȯ��
            if (trainee.isPlayer)
            {
                playerFinalRank = currentRank;
                yield return StartCoroutine(PlayerAnnounceSequence(trainee));
                // �÷��̾� ��ǥ �� RankingPanel ��Ȱ��ȭ, CharacterPanel Ȱ��ȭ ���� ����

                // �÷��̾� ��ǥ ��, ���� ������ �̵�
                currentRank--;
                continue;
            }

            // �Ϲ� ������ ���� ��ǥ
            yield return StartDialogueCoroutine(new string[] { $"������ {currentRank}�� �Դϴ�." });
            yield return new WaitForSeconds(1.0f); // ���尨 ����

            currentRank--;
        }
    }

    private void UpdateRankingList(TraineeData trainee, int rank)
    {
        // ��ŷ ��Ͽ� �ش� ������ �̸��� ǥ�� (VerticalLayoutGroup)
        GameObject newEntry = Instantiate(rankingEntryPrefab, rankingListParent);
        // TMP ������Ʈ�� ã�� ������ �̸� �Ҵ� (������ ������ �°� ���� �ʿ�)
        newEntry.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"{rank}��: {trainee.traineeName}";
    }

    // -----------------------------------------------------------
    // �÷��̾� ��ǥ ������
    // -----------------------------------------------------------

    private IEnumerator PlayerAnnounceSequence(TraineeData player)
    {
        // RankingPanel (���� ���) ��Ȱ��ȭ
        rankingPanelGroup.SetActive(false);

        // CharacterPanel Ȱ��ȭ �� ������ �Ҵ�
        characterPanel.SetActive(true);
        charRankText.text = player.rank.ToString();
        charNameText.text = player.traineeName;
        charAgencyText.text = player.agency;
        charVotesText.text = player.votes.ToString("N0") + "ǥ";
        charImage.sprite = player.characterImage;

        yield return StartDialogueCoroutine(new string[]
        {
            $"��ΰ� �ָ��ϰ� �ֽ��ϴ�! {player.traineeName} ������!",
            $"{player.traineeName} �������� ���� {player.rank}���� �����մϴ�!"
        });

        yield return new WaitForSeconds(2.0f);
    }

    // -----------------------------------------------------------
    // 3�� ��ǥ (Division Panel ���)
    // -----------------------------------------------------------

    private IEnumerator AnnounceFinalRound()
    {
        // 1. 2�� ��ǥ
        TraineeData secondPlace = allTrainees.Find(t => t.rank == 2);
        yield return StartDialogueCoroutine(new string[] { "���� ���� ������, 2���� ��ǥ�մϴ�." });
        UpdateRankingList(secondPlace, 2);
        yield return new WaitForSeconds(1.5f);

        // 2. 1�� ��ǥ (�÷��̾��� ���� ����)
        TraineeData firstPlace = allTrainees.Find(t => t.rank == 1);
        if (firstPlace.isPlayer)
        {
            playerFinalRank = 1;
            yield return StartCoroutine(PlayerAnnounceSequence(firstPlace));
        }
        else
        {
            yield return StartDialogueCoroutine(new string[] { "����� 1��! ���� ���� �ɱ��?" });
            UpdateRankingList(firstPlace, 1);
            yield return new WaitForSeconds(1.5f);
        }

        // 3. Division Panel Ȱ��ȭ �� 6��~3�� �̹��� �Ҵ�
        divisionPanel.SetActive(true);
        yield return StartDialogueCoroutine(new string[] { "���� 6������ 3�������� �������� �����մϴ�!" });

        var divisionTrainees = allTrainees.Where(t => t.rank >= 3 && t.rank <= 6)
            .OrderByDescending(t => t.rank) // 6������ ���������� ó��
            .ToList();

        for (int i = 0; i < divisionTrainees.Count; i++)
        {
            if (i < divisionImages.Length)
            {
                divisionImages[i].sprite = divisionTrainees[i].characterImage;
                divisionImages[i].gameObject.SetActive(true);
            }
        }

        // 4. 3�� ��ǥ (Division Panel�� �̹��� �Ҵ� �� 3�� ��ǥ)
        TraineeData thirdPlace = allTrainees.Find(t => t.rank == 3);
        yield return StartDialogueCoroutine(
            new string[] { $"���� ������, ������ ��� {thirdPlace.traineeName} ������!" });
        UpdateRankingList(thirdPlace, 3);

        yield return new WaitForSeconds(2.0f);
    }

    // -----------------------------------------------------------
    // ���� ������ (Ż�� ó��)
    // -----------------------------------------------------------

    private IEnumerator FinalizeResults()
    {
        // �÷��̾ ������ ���� (Ż���߰�), Ż�� ��ǥ�� �ʿ��� ���
        if (playerTrainee.rank > startingRankIndex || playerFinalRank == -1)
        {
            yield return StartDialogueCoroutine(new string[]
            {
                "�ƽ��Ե� �̹� ��ǥ������ �̸��� �ø��� ���߽��ϴ�.",
                $"{playerTrainee.traineeName} �������� �ƽ��� Ż���߽��ϴ�..."
            });

            gameOverPanel.SetActive(true); // ���� ���� �г� Ȱ��ȭ
        }
        else
        {
            yield return StartDialogueCoroutine(new string[] { "��ǥ�� ��� ���ƽ��ϴ�. �����մϴ�." });
        }
    }

    // -----------------------------------------------------------
    // GMManager ���� ����� (GMManager�� DontDestroyOnLoad �Ǿ� �־�� ��)
    // -----------------------------------------------------------

    private IEnumerator StartDialogueCoroutine(string[] dialogue)
    {
        if (GMManager.Instance == null)
        {
            Debug.LogError("GMManager �ν��Ͻ��� ã�� �� �����ϴ�.");
            yield break;
        }

        bool waitingForDialogue = true;

        // GMManager�� �ݹ��� �����Ͽ� ��ȭ ���� �� waitingForDialogue�� false�� ����
        GMManager.Instance.StartDialogue(dialogue, () => waitingForDialogue = false);

        // ��ȭ�� ���� ������ ���
        while (waitingForDialogue)
        {
            yield return null;
        }
    }
}