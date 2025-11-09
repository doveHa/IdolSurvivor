using UnityEngine;
using UnityEngine.UI;
using System;
using Script;
using Script.DataDefinition.Enum;
using Script.Manager;
using UnityEngine.SceneManagement;

public class PositionSelectManager : MonoBehaviour
{
    private PositionResultData positionResult;
    private LineDistributionData distributionResult;

    // 포지션 결정에 필요한 데이터 목록
    private PositionResultData[] positionData = new PositionResultData[]
    {
        new PositionResultData { checkResult = DiceCheckResult.CriticalSuccess, positionName = "센터", voteRatio = 0.5f },
        new PositionResultData { checkResult = DiceCheckResult.Success, positionName = "메보", voteRatio = 0.3f },
        new PositionResultData { checkResult = DiceCheckResult.Failure, positionName = "서브/래퍼", voteRatio = 0.1f }
    };

    // 분량 결정에 필요한 데이터 목록 (주사위 결과 3(Normal)을 중간으로 사용)
    private LineDistributionData[] distributionData = new LineDistributionData[]
    {
        new LineDistributionData { checkResult = DiceCheckResult.CriticalSuccess, distributionLabel = "많음", eventCount = 5 },
        new LineDistributionData { checkResult = DiceCheckResult.Success, distributionLabel = "중간", eventCount = 3 },
        new LineDistributionData { checkResult = DiceCheckResult.Failure, distributionLabel = "적음", eventCount = 1 }
    };

    void Start()
    {
        StartInitialDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartInitialDialogue()
    {
        string[] initialExplanation = new string[]
        {
            "이제 포지션을 정해볼까?",
            "혹시 센터 하고 싶은 사람 있어?",
            "...",
            "모두 센터를 원하고 있네.",
            "그럼 주사위를 굴려서 포지션을 정해볼까?"
        };

        if (GMManager.Instance != null)
        {
            // 대화가 끝난 후 포지션 주사위를 굴리도록 콜백 연결
            GMManager.Instance.StartDialogue(initialExplanation, RollPositionDice);
        }
    }

    private void RollPositionDice()
    {
        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.ShowDicePanel();

            Button rollBtn = DiceRoller.Instance.rollButton;
            if (rollBtn != null)
            {
                rollBtn.onClick.RemoveAllListeners(); 

                rollBtn.onClick.AddListener(() =>
                {
                    DiceRoller.Instance.RollDiceWithCallback(ProcessPositionResult);
                    rollBtn.interactable = false;
                });

                rollBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Roll";
                rollBtn.interactable = true;
            }
        }
    }

    private void ProcessPositionResult(int roll)
    {
        DiceCheckResult check = JudgeRollResult(roll);

        positionResult = Array.Find(positionData, d => d.checkResult == check);

        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.resultText.text =
                $"[D{roll}] {check}!\n포지션은 {positionResult.positionName}으로 결정되었습니다.";

            DiceRoller.Instance.SetRollCompletedUI();
            //DiceRoller.Instance.onNextAction = StartDistributionDialogue;
            DiceRoller.Instance.onNextAction = StartDistributionDialogue;
        }

        Debug.Log($"포지션 결정: {positionResult.positionName}, 득표 비율: {positionResult.voteRatio}");
    }

    private void StartDistributionDialogue()
    {
        string[] dialogue = new string[]
        {
            "포지션을 정했으니 이제 파트를 나눠보자.",
            "주사위를 굴려 이번 경연에서 받을 분량(이벤트 수)을 정합니다."
        };

        if (GMManager.Instance != null)
        {
            // 대화가 끝난 후 분량 주사위를 굴리도록 콜백 연결
            GMManager.Instance.StartDialogue(dialogue, RollDistributionDice);
        }
    }

    private void RollDistributionDice()
    {
        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.ShowDicePanel();

            Button rollBtn = DiceRoller.Instance.rollButton;
            if (rollBtn != null)
            {
                rollBtn.onClick.RemoveAllListeners();

                rollBtn.onClick.AddListener(() =>
                {
                    DiceRoller.Instance.RollDiceWithCallback(ProcessDistributionResult);
                    rollBtn.interactable = false;
                });

                rollBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Roll";
                rollBtn.interactable = true;
            }
        }
    }

    private void ProcessDistributionResult(int roll)
    {
        DiceCheckResult check = JudgeRollResult(roll);

        distributionResult = Array.Find(distributionData, d => d.checkResult == check);

        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.resultText.text =
                $"[D{roll}] {check}!\n분량은 {distributionResult.distributionLabel} ({distributionResult.eventCount}개)로 결정되었습니다.";

            DiceRoller.Instance.SetRollCompletedUI();

            DiceRoller.Instance.onNextAction = () =>
            {
                Debug.Log("모든 결정 완료! 다음 단계로...");
                SceneManager.LoadScene("Practice");
            };
        }

        Debug.Log($"분량 결정: {distributionResult.distributionLabel}, 이벤트 수: {distributionResult.eventCount}");

        //공연에 이벤트 수 반영
        Config.Event.EventCount = distributionResult.eventCount;
        Debug.Log($"Config.Event.EventCount가 {Config.Event.EventCount}로 설정되었습니다.");
    }

    private DiceCheckResult JudgeRollResult(int roll)
    {
        if (roll == 6) return DiceCheckResult.CriticalSuccess;
        if (roll == 4 || roll == 5) return DiceCheckResult.Success;
        return DiceCheckResult.Failure;
    }
}
