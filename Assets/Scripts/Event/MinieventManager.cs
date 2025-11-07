using UnityEngine;
using System;
using Script; // DiceRoller 네임스페이스
using Script.DataDefinition.Enum;
using UnityEngine.UI;
using TMPro; // TMPro 네임스페이스

public class MinieventManager : MonoBehaviour
{
    public enum EventType { PR_1Min, Practice, StreetPerformance, Entertainment }

    [Header("Event Settings")]
    public EventType currentEvent = EventType.PR_1Min;

    [Header("Street Performance UI")]
    public GameObject statSelectionPanel; // Inspector에서 할당 필요

    public StatButton[] statButtons;

    // 굴림 상태 관리 필드
    private int[] currentRolls;       // 현재 이벤트의 모든 주사위 결과를 저장
    private int requiredRolls = 0;    // 필요한 총 굴림 횟수
    private int currentRollIndex = 0; // 현재 굴림 횟수 인덱스
    private Action<int[]> onRollsCompleteCallback; // 모든 굴림 완료 후 실행될 콜백

    // 이벤트 결과 필드
    private int firstRoll = 0;
    private int finalVotesResult = 0;
    private int practicePoints = 0;
    private StatType selectedStatForStreet;

    // 길거리 공연 스탯 변화량 상수
    private const float StreetSuccessMultiplier = 3.0f;
    private const float StreetFailureReduction = 0.20f; // 20% 감소

    void Start()
    {
        StartCurrentEvent();
    }

    private void StartCurrentEvent()
    {
        // StartCurrentEvent가 호출될 때마다 해당 이벤트가 시작되도록 처리
        switch (currentEvent)
        {
            case EventType.PR_1Min:
                StartOneMinPR();
                break;
            case EventType.Practice:
                StartPractice();
                break;
            case EventType.StreetPerformance:
                StartStreetPerformance();
                break;
            case EventType.Entertainment:
                ProcessEntertainment();
                break;
        }
    }

    private string GetKoreanStatName(StatType type)
    {
        // 모든 StatType에 대해 필요한 번역을 추가하세요.
        switch (type)
        {
            case StatType.Sing:
                return "노래";
            case StatType.Dance:
                return "춤";
            case StatType.Charm:
                return "매력";
            case StatType.Appearance:
                return "외모";
            default:
                return type.ToString();
        }
    }

    // =================================================================
    // 범용 멀티 롤 시스템 (효율화의 핵심)
    // =================================================================

    /// <summary>
    /// N번의 주사위를 굴리고 결과를 배열에 저장한 후, 최종 콜백을 실행하는 범용 함수입니다.
    /// </summary>
    private void StartMultiRoll(int numRolls, Action<int[]> finalCallback)
    {
        requiredRolls = numRolls;
        currentRollIndex = 0;
        currentRolls = new int[numRolls];
        onRollsCompleteCallback = finalCallback;

        // 첫 번째 굴림을 시작하도록 UI 설정 및 리스너 연결
        ShowDiceAndSetRollListener();
        DiceRoller.Instance.rollButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"Roll (1/{numRolls})";
        DiceRoller.Instance.rollButton.interactable = true;
    }

    private void ShowDiceAndSetRollListener()
    {
        DiceRoller.Instance.ShowDicePanel();
        Button rollBtn = DiceRoller.Instance.rollButton;

        if (rollBtn != null)
        {
            rollBtn.onClick.RemoveAllListeners();
            rollBtn.onClick.AddListener(() =>
            {
                // 굴림이 끝난 후 NextDiceOrFinalize를 호출하도록 요청
                DiceRoller.Instance.RollDiceWithCallback(NextDiceOrFinalize);
                rollBtn.interactable = false;
            });
        }
    }

    private void NextDiceOrFinalize(int rollResult)
    {
        currentRolls[currentRollIndex] = rollResult;
        currentRollIndex++;

        if (currentRollIndex < requiredRolls)
        {
            // 굴릴 주사위가 남은 경우: 다음 굴림 준비 (Next 버튼으로 전환)
            DiceRoller.Instance.resultText.text =
                $"굴림 #{currentRollIndex} 결과: {rollResult}!\n다음 굴림을 준비하세요.";

            DiceRoller.Instance.SetRollCompletedUI(); // Roll -> Next 버튼 전환

            DiceRoller.Instance.onNextAction = () =>
            {
                // Next 버튼 클릭 시 다음 주사위 굴림 버튼 텍스트 설정 및 리스너 재할당
                ShowDiceAndSetRollListener();
                DiceRoller.Instance.rollButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
                    $"Roll ({currentRollIndex + 1}/{requiredRolls})";
            };
        }
        else
        {
            // 모든 굴림 완료: 최종 처리 시작
            DiceRoller.Instance.resultText.text = $"모든 굴림 완료! 최종 결과를 확인하세요.";
            DiceRoller.Instance.SetRollCompletedUI(); // Roll -> Next 버튼 전환

            // Next 버튼에 최종 처리 콜백 연결
            DiceRoller.Instance.onNextAction = () =>
            {
                //DiceRoller.Instance.HideDicePanel();
                //onRollsCompleteCallback.Invoke(currentRolls); // 최종 처리 함수에 결과 배열 전달
                onRollsCompleteCallback.Invoke(currentRolls);
            };
        }
    }


    // =================================================================
    // 1분 PR 이벤트 (Roll 2회)
    // =================================================================

    private void StartOneMinPR()
    {
        string[] dialogue = { "미니 이벤트 '1분 PR'에 오신 것을 환영합니다!", "주사위를 두 번 굴려 득표수를 결정합니다." };
        if (GMManager.Instance != null)
        {
            // StartMultiRoll을 사용하여 RollFirstPRDice, RollSecondPRDice 함수를 제거함
            GMManager.Instance.StartDialogue(dialogue,
                () => StartMultiRoll(2, ProcessFinalPRResult));
        }
    }

    private void ProcessFinalPRResult(int[] rolls)
    {
        firstRoll = rolls[0];
        int secondRoll = rolls[1];
        int finalVotes = 0;
        string formula = "";

        // 득표수 계산 로직
        if (firstRoll <= 3 && secondRoll <= 3)
        {
            finalVotes = (int)Mathf.Pow(firstRoll, secondRoll);
            formula = $"{firstRoll}^{secondRoll} = {finalVotes}";
        }
        else if (firstRoll % 2 == 0 && secondRoll % 2 == 0)
        {
            finalVotes = firstRoll * secondRoll;
            formula = $"{firstRoll} x {secondRoll} = {finalVotes}";
        }
        else if (firstRoll % 2 != 0 && secondRoll % 2 != 0)
        {
            finalVotes = firstRoll * secondRoll;
            formula = $"{firstRoll} x {secondRoll} = {finalVotes}";
        }
        else
        {
            finalVotes = firstRoll + secondRoll;
            formula = $"{firstRoll} + {secondRoll} = {finalVotes}";
        }

        finalVotesResult = finalVotes;

        // 득표수 적용 여기에

        FinalPRDialogue();
    }

    private void FinalPRDialogue()
    {
        string[] dialogue = { $"1분 PR이 끝났습니다. 득표수 {finalVotesResult}표를 획득하였습니다.", "다음 이벤트로 이동합니다." };
        if (GMManager.Instance != null)
        {
            GMManager.Instance.StartDialogue(dialogue, () => Debug.Log("미니 이벤트 끝. 다음 씬으로 전환"));
        }
    }


    // =================================================================
    // 연습 이벤트 (Roll 2회 -> Stat 분배)
    // =================================================================

    private void StartPractice()
    {
        string[] dialogue = { "연습 이벤트를 시작합니다. 주사위 합만큼 스탯 포인트를 얻고 분배합니다." };
        if (GMManager.Instance != null)
        {
            GMManager.Instance.StartDialogue(dialogue,
                () => StartMultiRoll(2, ProcessPracticeResult));
        }
    }

    private void ProcessPracticeResult(int[] rolls)
    {
        practicePoints = rolls[0] + rolls[1];

        // Next 버튼을 누르면 StatAllocationManager 호출
        StartPracticeAllocation();
    }

    private void StartPracticeAllocation()
    {
        if (DiceRoller.Instance != null) DiceRoller.Instance.HideDicePanel();

        if (StatAllocationManager.Instance != null)
        {
            // 분배가 끝난 후 FinalPracticeDialogue 함수를 실행하도록 콜백 전달
            StatAllocationManager.Instance.StartAllocation(practicePoints, FinalPracticeDialogue);
        }
        else
        {
            Debug.LogError("StatAllocationManager 인스턴스를 찾을 수 없습니다! 최종 대화로 바로 이동합니다.");
            FinalPracticeDialogue();
        }
    }

    private void FinalPracticeDialogue()
    {
        string[] dialogue = { $"연습이 끝났습니다. 총 {practicePoints} 포인트를 스탯에 분배했습니다.", "다음 이벤트로 이동합니다." };
        if (GMManager.Instance != null)
        {
            GMManager.Instance.StartDialogue(dialogue, () => Debug.Log("연습 이벤트 종료"));
        }
    }


    // =================================================================
    // 길거리 공연 이벤트 (스탯 선택 및 Roll 1회)
    // =================================================================

    private void StartStreetPerformance()
    {
        string[] dialogue = { "프로그램을 홍보하기 위해 길거리 공연 이벤트를 진행합니다!",
            "어떤 포지션으로 길거리 공연 이벤트에 나가시겠습니까?"};
        
        if (GMManager.Instance != null)
        {
            // 대화가 끝난 후 스탯 선택 UI 활성화
            GMManager.Instance.StartDialogue(dialogue, ShowStatSelectionUI);
        }
    }

    private void ShowStatSelectionUI()
    {
        if (statSelectionPanel != null)
        {
            // 스탯 선택 패널을 활성화하고 GM 패널을 닫습니다.
            statSelectionPanel.SetActive(true);
            if (GMManager.Instance != null)
            {
                GMManager.Instance.gmPanel.SetActive(false);
            }

            ConnectStatButtons();
        }
    }

    private void ConnectStatButtons()
    {
        foreach (var statBtn in statButtons)
        {
            if (statBtn != null && statBtn.GetComponent<Button>() != null)
            {
                Button btn = statBtn.GetComponent<Button>();

                // 기존 리스너 제거 (필수)
                btn.onClick.RemoveAllListeners();

                // AddListener를 사용해 람다 함수로 동적 연결
                btn.onClick.AddListener(() => OnStatSelectedByCode(statBtn.statType));
            }
        }
    }

    public void OnStatSelectedByCode(StatType type)
    {
        // 2. 선택된 스탯 저장
        selectedStatForStreet = type;

        if (GMManager.Instance != null)
        {
            string translatedStat = GetKoreanStatName(type);

            // 3. GM 패널에 확인 메시지 표시
            GMManager.Instance.gmText.text =
                $"{translatedStat} 스탯을 선택하시겠습니까?\n" +
                "(성공 시 스탯 증가, 실패 시 스탯 감소)";
            GMManager.Instance.gmPanel.SetActive(true);

            // 4. Next 버튼에 주사위 굴림 시작 함수 연결
            GMManager.Instance.NextBtn.onClick.RemoveAllListeners();
            GMManager.Instance.NextBtn.onClick.AddListener(RollStreetPerformanceDice);
        }
    }


    private void RollStreetPerformanceDice()
    {
        // 1. 스탯 선택 UI 및 GM 확인 패널 비활성화
        if (statSelectionPanel != null)
            statSelectionPanel.SetActive(false);
        if (GMManager.Instance != null)
            GMManager.Instance.gmPanel.SetActive(false);

        if (DiceRoller.Instance != null)
        {
            // 2. 주사위 패널 열기
            DiceRoller.Instance.ShowDicePanel();

            Button rollBtn = DiceRoller.Instance.rollButton;
            if (rollBtn != null)
            {
                // 3. Roll 버튼 리스너 설정: 굴림 완료 후 ProcessStreetResult 실행
                rollBtn.onClick.RemoveAllListeners();
                rollBtn.onClick.AddListener(() =>
                {
                    // 수제 구현 핵심: RollDiceWithCallback에 ProcessStreetResult 연결
                    DiceRoller.Instance.RollDiceWithCallback(ProcessStreetResultManual);
                    rollBtn.interactable = false;
                });

                // UI 텍스트 설정
                rollBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Roll";
                rollBtn.interactable = true;
            }
        }
    }

    private void ProcessStreetResultManual(int roll)
    {
        // ProcessStreetResult 로직을 직접 실행합니다.
        string translatedStat = GetKoreanStatName(selectedStatForStreet);

        // 1. 스탯 값 가져오기 (임시)
        int baseValue = 10;

        DiceCheckResult check = JudgeStreetRoll(roll);
        string resultMessage;
        int change = 0;

        // 2. 성공/실패 판정 및 스탯 변경 계산
        if (check == DiceCheckResult.Success)
        {
            change = (int)(baseValue * StreetSuccessMultiplier);
            resultMessage =
                $"[굴림: {roll}] 성공!\n" +
                $"스탯이 {StreetSuccessMultiplier}배 증가하여 {change}가 되었습니다!";
            // TODO: PlayerManager.Instance.ApplyStatChange(selectedStatForStreet, change);
        }
        else
        {
            int reductionAmount = (int)(baseValue * StreetFailureReduction); // 10 * 0.20f = 2
            change = -reductionAmount;

            resultMessage =
                $"[굴림: {roll}] 실패!\n" +
                $"스탯이 20% 감소하여 {change}가 되었습니다.";
            // TODO: PlayerManager.Instance.ApplyStatChange(selectedStatForStreet, reduction);
        }

        // 3. UI 업데이트 및 Next 버튼 설정
        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.resultText.text = resultMessage;
            DiceRoller.Instance.SetRollCompletedUI();

            // 4. Next 버튼 클릭 시 FinalStreetDialogue 실행
            DiceRoller.Instance.onNextAction = FinalStreetDialogue;
        }
    }

    private void FinalStreetDialogue()
    {
        Debug.Log("FinalStreetDialogue 호출됨");

        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.HideDicePanel();
        }

        string[] dialogue = new string[]
        {
            "길거리 공연 이벤트가 끝났습니다.",
            "다음 이벤트로 이동합니다."
        };

        if (GMManager.Instance != null)
        {
            GMManager.Instance.StartDialogue(dialogue, FinalActionOfStreetEvent);
        }
    }

    private void FinalActionOfStreetEvent()
    {
        Debug.Log("길거리 공연 이벤트 종료");
        // 여기에 씬 전환 또는 다음 이벤트 시작 로직을 넣습니다.
    }

    // 길거리 공연 판정 로직 (1~3: 실패, 4~6: 성공)
    private DiceCheckResult JudgeStreetRoll(int roll)
    {
        if (roll >= 4) return DiceCheckResult.Success;
        return DiceCheckResult.Failure;
    }


    private void ProcessEntertainment()
    {
        int guaranteedVotes = 21;
        string[] dialogue = new string[]
        {
            "PPL 예능 코너에서 활약했습니다.",
            $"득표수 {guaranteedVotes}표를 얻었습니다.\n다음 단계로 이동합니다."
        };

        if (GMManager.Instance != null)
        {
            GMManager.Instance.StartDialogue(dialogue, () =>
            {
                // TeamManager.Instance.AddVotes(guaranteedVotes);
                Debug.Log("예능 이벤트 끝. 다음 씬으로 전환");
            });
        }
    }
}