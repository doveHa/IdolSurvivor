using Script;
using UnityEngine;
using System;
using Script.DataDefinition.Enum;
using UnityEngine.UI;

public class MinieventManager : MonoBehaviour
{
    public enum EventType { PR_1Min, Practice, StreetPerformance, Entertainment }

    public EventType currentEvent = EventType.PR_1Min;

    private int rollCount = 0;
    private int firstRoll = 0;
    private int finalVotesResult = 0;

    void Start()
    {
        // 씬 시작 시 바로 이벤트 시작
        StartCurrentEvent();
    }

    private void StartCurrentEvent()
    {
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

    private void StartOneMinPR()
    {
        string[] dialogue = new string[]
        {
            "미니 이벤트 '1분 PR'에 오신 것을 환영합니다!",
            "1분 PR은 주사위를 두 번 굴려 득표수를 결정합니다.",
            "3 이하의 숫자가 2개면 거듭제곱,\n짝수 2개거나 홀수 2개면 곱하기,",
            "나머지는 더하기로 득표수를 산정합니다."
        };

        if (GMManager.Instance != null)
        {
            // 대화가 끝난 후 첫 번째 주사위 굴림 시작
            GMManager.Instance.StartDialogue(dialogue, RollFirstPRDice);
        }
    }

    // 좀 더 효율적이고 코드를 깔끔하게 쓸 수 있을 방법이 있을거 같은데 고민...
    private void RollFirstPRDice()
    {
        // 첫 번째 굴림 설정
        rollCount = 1;

        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.ShowDicePanel();

            Button rollBtn = DiceRoller.Instance.rollButton;
            if (rollBtn != null)
            {
                rollBtn.onClick.RemoveAllListeners();

                // 첫 번째 굴림의 콜백은 ProcessFirstPRRoll
                rollBtn.onClick.AddListener(() =>
                {
                    DiceRoller.Instance.RollDiceWithCallback(ProcessFirstPRRoll);
                    rollBtn.interactable = false;
                });

                rollBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Roll (1/2)";
                rollBtn.interactable = true;
            }
        }
    }

    private void ProcessFirstPRRoll(int roll)
    {
        firstRoll = roll; // 첫 번째 결과 저장

        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.resultText.text = $"첫 번째: {roll}!\n다음 굴림을 준비하세요.";
            DiceRoller.Instance.SetRollCompletedUI(); // 버튼을 Next로 변경

            // Next 버튼을 누르면 두 번째 주사위 굴림 시작
            DiceRoller.Instance.onNextAction = RollSecondPRDice;
        }
    }

    private void RollSecondPRDice()
    {
        // 두 번째 굴림 설정
        rollCount = 2;

        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.ShowDicePanel(); // 패널이 닫혔다면 다시 열고, 

            Button rollBtn = DiceRoller.Instance.rollButton;
            if (rollBtn != null)
            {
                rollBtn.onClick.RemoveAllListeners();
                rollBtn.onClick.AddListener(() =>
                {
                    // 두 번째 굴림의 콜백은 ProcessFinalPRResult
                    DiceRoller.Instance.RollDiceWithCallback(ProcessFinalPRResult);
                    rollBtn.interactable = false;
                });

                rollBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Roll (2/2)";
                rollBtn.interactable = true;
            }
        }
    }

    private void ProcessFinalPRResult(int secondRoll)
    {
        int finalVotes = 0;
        string formula = "";

        // 득표수 계산 로직
        if (firstRoll <= 3 && secondRoll <= 3)
        {
            // 3이하 2개 거듭제곱 3 * 3
            finalVotes = (int)Mathf.Pow(firstRoll, secondRoll);
            formula = $"{firstRoll}^{secondRoll} = {finalVotes}";
        }
        else if (firstRoll % 2 == 0 && secondRoll % 2 == 0)
        {
            // 짝수, 짝수 곱하기
            finalVotes = firstRoll * secondRoll;
            formula = $"{firstRoll} x {secondRoll} = {finalVotes}";
        }
        else if (firstRoll % 2 != 0 && secondRoll % 2 != 0)
        {
            // 홀수, 홀수 곱하기
            finalVotes = firstRoll * secondRoll;
            formula = $"{firstRoll} x {secondRoll} = {finalVotes}";
        }
        else
        {
            // 나머지 더하기
            finalVotes = firstRoll + secondRoll;
            formula = $"{firstRoll} + {secondRoll} = {finalVotes}";
        }

        finalVotesResult = finalVotes;

        // 4. UI 업데이트 및 다음 단계 연결
        if (DiceRoller.Instance != null)
        {
            string message = $"최종 득표 결과: {finalVotes}표!\n(계산식: {formula})";
            Debug.Log(finalVotes);

            DiceRoller.Instance.resultText.text = message;
            DiceRoller.Instance.SetRollCompletedUI();

            // 득표수 적용 여기에

            // Next 버튼 누르면 최종 대사 시작
            DiceRoller.Instance.onNextAction = FinalPRDialogue;
        }
    }

    private void FinalPRDialogue()
    {
        string[] dialogue = new string[]
        {
            $"1분 PR이 끝났습니다. 득표수 {finalVotesResult}표를 획득하였습니다.", // 실제 득표수는 finalVotes 변수에 있음
            "다음 이벤트로 이동합니다."
        };

        if (GMManager.Instance != null)
        {
            // 최종 대화가 끝난 후 씬 전환 또는 다음 이벤트 시작
            GMManager.Instance.StartDialogue(dialogue, () => Debug.Log("미니 이벤트 끝. 다음 씬으로 전환"));
        }
    }

    private void StartPractice()
    {
        string[] dialogue = new string[]
        {
            "연습을 시작합니다.",
            "주사위를 두 번 굴려 나온 합만큼 스탯 포인트를 얻습니다.",
            "이 포인트는 플레이어의 스탯에 자유롭게 분배할 수 있습니다."
        };

        if (GMManager.Instance != null)
        {
            // 대화가 끝난 후 두 번의 주사위 굴림을 위한 함수 호출
            GMManager.Instance.StartDialogue(dialogue, RollTwoDiceForSum);
        }
    }

    private void RollTwoDiceForSum()
    {
        // 간단화를 위해 바로 최종 콜백으로 연결
        rollCount = 0;
        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.ShowDicePanel();
            DiceRoller.Instance.rollButton.onClick.RemoveAllListeners();
            DiceRoller.Instance.rollButton.onClick.AddListener(() =>
            {
                // ProcessTwoDiceSum을 콜백으로 넘기기
                DiceRoller.Instance.RollDiceWithCallback(roll1 => RollDiceForSumSecond(roll1));
                DiceRoller.Instance.rollButton.interactable = false;
            });
            DiceRoller.Instance.rollButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Roll (1/2)";
            DiceRoller.Instance.rollButton.interactable = true;
        }
    }

    private void RollDiceForSumSecond(int roll1)
    {
        // 첫 번째 굴림 후 Next 버튼 로직 처리
        DiceRoller.Instance.resultText.text = $"첫 번째: {roll1}. 다음 굴림을 준비하세요.";
        DiceRoller.Instance.SetRollCompletedUI();
        DiceRoller.Instance.onNextAction = () =>
        {
            // Next 버튼 클릭 후 두 번째 주사위 굴림 시작
            DiceRoller.Instance.ShowDicePanel();
            Button rollBtn = DiceRoller.Instance.rollButton;
            rollBtn.onClick.RemoveAllListeners();
            rollBtn.onClick.AddListener(() =>
            {
                DiceRoller.Instance.RollDiceWithCallback(roll2 => ProcessTwoDiceSum(roll1, roll2));
                rollBtn.interactable = false;
            });
            rollBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Roll (2/2)";
            rollBtn.interactable = true;
        };
    }

    private void ProcessTwoDiceSum(int roll1, int roll2)
    {
        int totalPoints = roll1 + roll2;

        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.resultText.text =
                $"주사위 합: {roll1} + {roll2} = {totalPoints} 포인트 획득!.";
            DiceRoller.Instance.SetRollCompletedUI();

            // Next 버튼을 누르면 스탯 분배 UI로 이동 (가정)
            DiceRoller.Instance.onNextAction = () =>
            {
                Debug.Log($"총 {totalPoints} 포인트를 분배할 수 있습니다. (스탯 분배 UI 구현 필요)");
                // ShowStatDistributionUI(totalPoints); // 별도 UI 함수 호출

                // 여기서 스탯 추가

                // 분배가 끝났다고 가정하고 최종 대화 시작
                FinalPracticeDialogue(totalPoints);
            };
        }
    }

    private void FinalPracticeDialogue(int points)
    {
        string[] dialogue = new string[]
        {
            $"연습이 끝났습니다. 총 {points} 포인트를 스탯에 분배했습니다.",
            "다음 이벤트로 이동합니다."
        };
        GMManager.Instance.StartDialogue(dialogue, () => Debug.Log("연습 이벤트 종료"));
    }

    private void StartStreetPerformance()
    {
        string[] dialogue = new string[]
        {
            "프로그램을 홍보하기 위해 길거리 공연을 진행 합니다."
        };

        if (GMManager.Instance != null)
        {
            GMManager.Instance.StartDialogue(dialogue, () =>
            {
                // TeamManager.Instance.StartStreetPerformanceEvent();
                Debug.Log("길거리 공연 이벤트 시작");
            });
        }
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
