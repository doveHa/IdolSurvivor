using Script;
using Script.DataDefinition.Enum;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Script.Manager;
using Script.Characters;

public class MinieventManager2 : MonoBehaviour
{
    public enum CrisisEventType
    {
        SNSActivity,        // 26%
        SpecialOffer,       // 10%
        SchoolViolence,     // 10%
        AlcoholTobacco,     // 27%
        TeamConflict        // 27%
    }



    [Header("UI References")]
    public GameObject specialOfferPanel; // 특혜 제안 선택지 UI (Inspector에서 할당)
    public Button acceptOfferButton;     // 특혜 제안 수락 버튼
    public Button declineOfferButton;    // 특혜 제안 거절 버튼
    public GameObject gameOverPanel;

    //임시
    private int tempStatValue = 100;
    private int tempTeamColor = 50;

    private enum DiceCheckResult { Failure, Success, Normal }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        StartCrisisEvent();
    }

    private readonly (CrisisEventType type, int weight)[] eventWeights = new[]
    {
        (CrisisEventType.SNSActivity, 26),
        (CrisisEventType.SpecialOffer, 10),
        (CrisisEventType.SchoolViolence, 10),
        (CrisisEventType.AlcoholTobacco, 27),
        (CrisisEventType.TeamConflict, 27)
    };

    private void StartCrisisEvent()
    {
        // 1. 랜덤으로 위기 이벤트 선택
        CrisisEventType selectedEvent = GetRandomCrisisEvent();
        //CrisisEventType selectedEvent = CrisisEventType.AlcoholTobacco; // 테스트용 고정

        // 2. 선택된 이벤트 시작
        switch (selectedEvent)
        {
            case CrisisEventType.SNSActivity:
                StartSNSActivity();
                break;
            case CrisisEventType.SpecialOffer:
                StartSpecialOffer();
                break;
            case CrisisEventType.SchoolViolence:
                StartSchoolViolence();
                break;
            case CrisisEventType.AlcoholTobacco:
                StartAlcoholTobacco();
                break;
            case CrisisEventType.TeamConflict:
                StartTeamConflict();
                break;
        }
    }

    private CrisisEventType GetRandomCrisisEvent()
    {
        int totalWeight = eventWeights.Sum(ew => ew.weight);
        int randValue = UnityEngine.Random.Range(1, totalWeight + 1);
        int cumulativeWeight = 0;
        foreach (var (type, weight) in eventWeights)
        {
            cumulativeWeight += weight;
            if (randValue <= cumulativeWeight)
            {
                Debug.Log($"랜덤 이벤트 선택됨: {type} (확률: {weight}%)");
                return type;
            }
        }

        // 안전 장치 (도달해서는 안 됨)
        return eventWeights[0].type;
    }

    // 주사위 판정
    private DiceCheckResult JudgeSuccessFailure(int roll)
    {
        if (roll >= 4) return DiceCheckResult.Success;
        return DiceCheckResult.Failure;
    }

    private DiceCheckResult JudgeSuccessNormalFailure(int roll)
    {
        if (roll >= 5) return DiceCheckResult.Success;
        if (roll >= 3) return DiceCheckResult.Normal;
        return DiceCheckResult.Failure;
    }

    // 범용 헬퍼 함수
    private void StartSingleRoll(Action<int> finalCallback, string rollButtonText)
    {
        if (DiceRoller.Instance == null)
        {
            Debug.LogError("DiceRoller 인스턴스를 찾을 수 없습니다.");
            return;
        }

        DiceRoller.Instance.ShowDicePanel();
        Button rollBtn = DiceRoller.Instance.rollButton;

        if (rollBtn != null)
        {
            rollBtn.onClick.RemoveAllListeners();
            rollBtn.onClick.AddListener(() =>
            {
                // 굴림 후 Next 버튼에 최종 콜백 연결
                DiceRoller.Instance.RollDiceWithCallback((rollResult) =>
                {
                    DiceRoller.Instance.SetRollCompletedUI();
                    DiceRoller.Instance.onNextAction = () => finalCallback.Invoke(rollResult);
                });
                rollBtn.interactable = false;
            });

            rollBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = rollButtonText;
            rollBtn.interactable = true;
        }
    }

    private void FinalizeCrisisEvent()
    {
        /*if (DiceRoller.Instance != null) DiceRoller.Instance.HideDicePanel();

        string[] finalDialogue = { "위기 이벤트가 종료되었습니다.", "다음 단계로 이동합니다." };*/

        Debug.Log("위기 이벤트 종료");
        Config.Team.TeamCount = 2;
        Config.Team.AllCharacterCount = 6;

        // 씬 로드 실행
        SceneManager.LoadScene("TeamBuildingScene");
    }

    private void ShowGameOver()
    {
        if (DiceRoller.Instance != null) DiceRoller.Instance.HideDicePanel();
        if (GMManager.Instance != null) GMManager.Instance.gmPanel.SetActive(false);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("게임 오버!");
        }

        else
        {
            Debug.LogError("Game Over Panel이 할당되지 않았습니다. 게임 오버 로직을 수동으로 처리해야 합니다.");
        }
    }

    // 이벤트 구현
    private void StartSNSActivity()
    {
        string[] dialogue = { "사건 발생! 프로그램의 규칙을 깨고 몰래 SNS를 업로드 하였습니다.", "주사위를 굴려 대응 성공 여부를 판정합니다. (성공/실패)" };

        GMManager.Instance.StartDialogue(dialogue,
            () => StartSingleRoll(ProcessSNSActivityResult, "Roll") // 주사위 굴림 시작
        );
    }

    #region SNS 활동
    private void ProcessSNSActivityResult(int roll)
    {
        DiceRoller.Instance.HideDicePanel();
        DiceCheckResult check = JudgeSuccessFailure(roll);

        string resultMessage;
        string[] finalDialogue;

        if (check == DiceCheckResult.Success)
        {
            // 성공: 득표수 100 획득
            resultMessage = "팬들에게 어필하여 득표수 100표를 획득했습니다.";
            finalDialogue = new string[] { $"결과: {roll} 성공!", resultMessage };
            AllCharacterManager.Manager.Player.AddVote(100);
        }
        else
        {
            // 실패: 팀 빌딩 불가 All 랜덤
            resultMessage = "규칙을 어긴 패널티로 팀 빌딩이 불가능해졌습니다.\n모든 스탯이 랜덤으로 재조정됩니다.";
            finalDialogue = new string[] { $"결과: {roll} 실패!", resultMessage };
            // TODO: TeamManager.Instance.RandomizeAllStats();
        }

        GMManager.Instance.StartDialogue(finalDialogue, FinalizeCrisisEvent);
    }
    #endregion

    #region 특혜 제안
    private void StartSpecialOffer()
    {
        string[] dialogue = { "소속사로부터 은밀한 제안이 들어왔습니다.", "투표수를 조작해 더 높은 순위를 얻을 수 있게 제안합니다. 수락하시겠습니까?" };
        GMManager.Instance.StartDialogue(dialogue, ShowSpecialOfferOptions);
    }

    private void ShowSpecialOfferOptions()
    {
        if (GMManager.Instance != null) GMManager.Instance.gmPanel.SetActive(false);
        if (specialOfferPanel != null)
        {
            specialOfferPanel.SetActive(true);

            // 버튼 리스너 연결
            acceptOfferButton.onClick.RemoveAllListeners();
            acceptOfferButton.onClick.AddListener(() => ProcessSpecialOffer(true));

            declineOfferButton.onClick.RemoveAllListeners();
            declineOfferButton.onClick.AddListener(() => ProcessSpecialOffer(false));
        }
    }

    private void ProcessSpecialOffer(bool accepted)
    {
        if (specialOfferPanel != null) specialOfferPanel.SetActive(false);

        if (accepted)
        {
            // 수락 시: GM 대사 후 주사위 굴림 시작
            string[] dialogue = { "소속사의 은밀한 제안을 수락했습니다.", "주사위를 굴려 특혜가 발각되지 않을지 판정합니다. (성공/실패, 성공률 50%)" };

            GMManager.Instance.StartDialogue(dialogue,
                // 주사위 굴림 시작, 결과는 ProcessSpecialOfferRollResult에서 처리
                () => StartSingleRoll(ProcessSpecialOfferRollResult, "Roll")
            );
        }
        else
        {
            // 거절: 넘어감
            string[] finalDialogue = new string[] { "소속사의 제안을 거절했습니다.", "정정당당하게 하겠습니다!" };
            GMManager.Instance.StartDialogue(finalDialogue, FinalizeCrisisEvent);
        }
    }

    private void ProcessSpecialOfferRollResult(int roll)
    {
        // DiceRoller의 Next 버튼 클릭 시 호출됩니다.
        if (DiceRoller.Instance != null) DiceRoller.Instance.HideDicePanel();

        DiceCheckResult check = JudgeSuccessFailure(roll);
        string resultMessage;
        string[] finalDialogue;
        Action finalAction;

        if (check == DiceCheckResult.Success)
        {
            // 성공: 원하는 포지션과 노래 바로 선택 가능
            resultMessage = "득표 수에서 100표가 추가되었습니다.";
            finalDialogue = new string[] { $"판정 결과: {roll} (성공)", "소속사의 특혜가 비밀리에 성공했습니다.", resultMessage };
            finalAction = FinalizeCrisisEvent;
            
            AllCharacterManager.Manager.Player.AddVote(100);
        }
        else
        {
            // 실패: 특혜가 걸렸다. 게임 오버
            resultMessage = "특혜가 걸린 것이 대중에 알려져 프로그램에서 하차합니다.";
            finalDialogue = new string[] { $"판정 결과: {roll} (실패)", "비밀이 폭로되었습니다!", resultMessage };
            finalAction = ShowGameOver;
        }

        // 최종 GM 대화 또는 게임 오버
        GMManager.Instance.StartDialogue(finalDialogue, finalAction);
    }

    #endregion

    #region 학폭 폭로
    private void StartSchoolViolence()
    {
        string[] dialogue = { "인터넷에 과거에 학교 폭력을 했다는 폭로글이 올라왔습니다.\n폭로글이 진실인지 판정합니다." };
        GMManager.Instance.StartDialogue(dialogue,
            () => StartSingleRoll(ProcessSchoolViolenceResult, "Roll")
        );
    }

    private void ProcessSchoolViolenceResult(int roll)
    {
        DiceRoller.Instance.HideDicePanel();
        string[] finalDialogue;

        // 1: 실패(진짜였음) → 게임 오버
        if (roll == 1)
        {
            finalDialogue = new string[] { $"진실 판정 결과: {roll}", "학폭이 사실로 드러났습니다. 프로그램에서 영구 하차합니다." };
            GMManager.Instance.StartDialogue(finalDialogue, ShowGameOver);
        }
        else
        {
            // 나머지 (2~6): 친구가 글 올림 → 넘어감
            finalDialogue = new string[] { $"진실 판정 결과: {roll}", "폭로글이 거짓으로 드러나 논란이 무사히 넘어갔습니다." };
            GMManager.Instance.StartDialogue(finalDialogue, FinalizeCrisisEvent);
        }
    }

    #endregion

    #region 술 담배 논란
    private void StartAlcoholTobacco()
    {
        string[] dialogue = { "팀원 중 한 명의 과거 술/담배 논란 사진이 유출되었습니다.", "주사위를 굴려 여론 수습 성공 여부를 판정합니다. (성공/실패)" };
        GMManager.Instance.StartDialogue(dialogue,
            () => StartSingleRoll(ProcessAlcoholTobaccoResult, "Roll")
        );
    }

    private void ProcessAlcoholTobaccoResult(int roll)
    {
        DiceRoller.Instance.HideDicePanel();
        DiceCheckResult check = JudgeSuccessFailure(roll);

        string resultMessage;
        string[] finalDialogue;
        float reductionRatio = 0.20f; // 20% 감소

        if (check == DiceCheckResult.Success)
        {
            // 성공: 넘어감
            resultMessage = $"성공! (Roll: {roll}) 팀원의 진실된 사과로 다행히 큰 문제가 되지 않았습니다.";
            finalDialogue = new string[] { resultMessage, "무사히 넘어갔습니다." };
        }
        else
        {
            // 실패: 스탯 감소
            int reduction = -20; // 스탯 감소량 임시 설정
            resultMessage = $"실패! (Roll: {roll}) 여론이 악화되었습니다. 모든 스탯이 {reduction} 감소했습니다.";
            finalDialogue = new string[] { "이미지 손상으로 활동에 제약이 생겼습니다.", resultMessage };

            ApplyStatChangeRatioAll(reductionRatio);
        }

        GMManager.Instance.StartDialogue(finalDialogue, FinalizeCrisisEvent);
    }

    #endregion

    #region 팀원 간 불화
    private void StartTeamConflict()
    {
        string[] dialogue = { "팀원들 사이에 사소한 불화가 발생했습니다.", "주사위를 굴려 불화 극복 결과를 판정합니다. (성공/보통/실패)" };
        GMManager.Instance.StartDialogue(dialogue,
            () => StartSingleRoll(ProcessTeamConflictResult, "Roll")
        );
    }

    private void ProcessTeamConflictResult(int roll)
    {
        DiceRoller.Instance.HideDicePanel();
        DiceCheckResult check = JudgeSuccessNormalFailure(roll);

        string resultMessage;
        string[] finalDialogue;

        if (check == DiceCheckResult.Success)
        {
            // 성공: 더 단합력 높아졌다. 팀 컬러 증가
            int gain = 10; // 팀 컬러 증가량 임시 설정
            resultMessage = $"성공! (Roll: {roll}) 위기를 극복하며 팀이 더 단단해졌습니다. 팀 컬러가 +{gain} 증가했습니다.";
            finalDialogue = new string[] { "우리는 하나!", resultMessage };
            tempTeamColor += gain; // 임시 변수 업데이트
            Debug.Log($"임시 팀 컬러 증가: {tempTeamColor}");
            // TODO: TeamManager.Instance.AddTeamColor(gain);
        }
        else if (check == DiceCheckResult.Normal)
        {
            // 보통: 넘어감
            resultMessage = $"보통. (Roll: {roll}) 불화는 있었지만, 잘 해결하고 넘어갔습니다.";
            finalDialogue = new string[] { resultMessage, "다음 활동에 집중합시다." };
        }
        else // Failure
        {
            // 실패: 팀 컬러 삭제
            resultMessage = $"실패! (Roll: {roll}) 불화가 걷잡을 수 없이 커져 팀 컬러가 삭제되었습니다.";
            finalDialogue = new string[] { "팀워크가 무너졌습니다.", resultMessage };
            tempTeamColor = 0; // 임시 변수 업데이트
            Debug.Log("임시 팀 컬러 삭제");
            // TODO: TeamManager.Instance.ResetTeamColor();
        }

        if (DiceRoller.Instance != null) DiceRoller.Instance.HideDicePanel();
        GMManager.Instance.StartDialogue(finalDialogue, FinalizeCrisisEvent);
    }
    #endregion

    private void ApplyStatChangeAll(int amount)
    {
        if (AllCharacterManager.Manager != null && AllCharacterManager.Manager.Player != null)
        {
            var playerStats = AllCharacterManager.Manager.Player.Stat;

            // CharacterStats의 AddStatValue를 사용하여 모든 스탯을 업데이트
            playerStats.AddStatValue(StatType.Sing, amount);
            playerStats.AddStatValue(StatType.Dance, amount);
            playerStats.AddStatValue(StatType.Appearance, amount);
            playerStats.AddStatValue(StatType.Charm, amount);

            Debug.Log($"[AlcoholTobacco] 모든 스탯에 {amount} 반영 완료. 최종 스탯: {playerStats.ToString()}");
        }
        else
        {
            Debug.LogError("AllCharacterManager나 Player 스탯 객체를 찾을 수 없어 일괄 변경을 적용할 수 없습니다.");
        }
    }

    private int GetPlayerStatValue(StatType type)
    {
        if (AllCharacterManager.Manager != null && AllCharacterManager.Manager.Player != null)
        {
            var playerStats = AllCharacterManager.Manager.Player.Stat;
            switch (type)
            {
                case StatType.Sing: return playerStats.Sing.Value;
                case StatType.Dance: return playerStats.Dance.Value;
                case StatType.Charm: return playerStats.Charm.Value;
                case StatType.Appearance: return playerStats.Appearance.Value;
            }
        }
        return 10;
    }

    private void ApplyStatChangeRatioAll(float ratio) // ratio: 0.20f (20%)
    {
        if (AllCharacterManager.Manager != null && AllCharacterManager.Manager.Player != null)
        {
            var playerStats = AllCharacterManager.Manager.Player.Stat;

            foreach (var stat in playerStats.ToStatArray())
            {
                // 현재 스탯 값의 비율만큼 감소량 계산
                int reductionAmount = (int)(stat.Value * ratio);

                // 감소는 음수(-)를 사용합니다.
                int finalChange = -reductionAmount;

                // AddStatValue(StatType, int)를 호출하여 변경 적용
                playerStats.AddStatValue(stat.StatType, finalChange);

                Debug.Log($"[AlcoholTobacco] {stat.StatType} 감소량: {reductionAmount}, 최종 값: {stat.Value}");
            }

            Debug.Log($"모든 스탯 20% 감소 완료. 최종 스탯: {playerStats.ToString()}");
        }
        else
        {
            Debug.LogError("AllCharacterManager나 Player 스탯 객체를 찾을 수 없어 비율 변경을 적용할 수 없습니다.");
        }
    }
}