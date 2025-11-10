using Script;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PracticeManager : MonoBehaviour
{
    public static PracticeManager Instance { get; private set; }

    private int firstRoll = 0;
    private int secondRoll = 0;
    private int thirdRoll = 0;
    private int totalDiceSum = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Start()
    {
<<<<<<< Updated upstream
        // 씬 시작 시 바로 연습 이벤트 시작
=======
        // �� ���� �� �ٷ� ���� �̺�Ʈ ����
>>>>>>> Stashed changes
        StartPractice();
    }

    public void StartPractice()
    {
        string[] dialogue = new string[]
        {
<<<<<<< Updated upstream
            "공연 연습 미니 이벤트를 시작합니다.",
            "주사위를 세 번 굴려 나온 합만큼 스탯 포인트를 얻고, 원하는 스탯에 분배합니다.",
            "지금부터 첫 번째 주사위를 굴립니다."
=======
            "���� ���� �̴� �̺�Ʈ�� �����մϴ�.",
            "�ֻ����� �� �� ���� ���� �ո�ŭ ���� ����Ʈ�� ���, ���ϴ� ���ȿ� �й��մϴ�.",
            "���ݺ��� ù ��° �ֻ����� �����ϴ�."
>>>>>>> Stashed changes
        };

        if (GMManager.Instance != null)
        {
            
            GMManager.Instance.StartDialogue(dialogue, RollFirstDice);
        }
        else
        {
            RollFirstDice();
        }
    }

    private void RollFirstDice()
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
                    DiceRoller.Instance.RollDiceWithCallback(ProcessFirstRoll);
                    rollBtn.interactable = false;
                });

                rollBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Roll (1/3)";
                rollBtn.interactable = true;
            }
        }
    }

    private void ProcessFirstRoll(int roll)
    {
        firstRoll = roll;

        if (DiceRoller.Instance != null)
        {
<<<<<<< Updated upstream
            DiceRoller.Instance.resultText.text = $"첫 번째: {firstRoll}!";
=======
            DiceRoller.Instance.resultText.text = $"ù ��°: {firstRoll}!";
>>>>>>> Stashed changes
            DiceRoller.Instance.SetRollCompletedUI();

            DiceRoller.Instance.onNextAction = RollSecondDice;
        }
    }

    private void RollSecondDice()
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
                    DiceRoller.Instance.RollDiceWithCallback(ProcessSecondRoll);
                    rollBtn.interactable = false;
                });

                rollBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Roll (2/3)";
                rollBtn.interactable = true;
            }
        }
    }

    private void ProcessSecondRoll(int roll)
    {
        secondRoll = roll;

        if (DiceRoller.Instance != null)
        {
<<<<<<< Updated upstream
            DiceRoller.Instance.resultText.text = $"두 번째: {secondRoll}!\n현재 합계: {firstRoll + secondRoll}";
=======
            DiceRoller.Instance.resultText.text = $"�� ��°: {secondRoll}!\n���� �հ�: {firstRoll + secondRoll}";
>>>>>>> Stashed changes
            DiceRoller.Instance.SetRollCompletedUI();

            DiceRoller.Instance.onNextAction = RollThirdDice;
        }
    }

    private void RollThirdDice()
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
                    DiceRoller.Instance.RollDiceWithCallback(ProcessThirdRoll);
                    rollBtn.interactable = false;
                });

                rollBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Roll (3/3)";
                rollBtn.interactable = true;
            }
        }
    }

    private void ProcessThirdRoll(int roll)
    {
        thirdRoll = roll;

        totalDiceSum = firstRoll + secondRoll + thirdRoll;

        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.resultText.text =
<<<<<<< Updated upstream
                $"세 번째: {thirdRoll}!\n최종 주사위 합계: {firstRoll} + {secondRoll} + {thirdRoll} = {totalDiceSum}포인트 획득!";
            DiceRoller.Instance.SetRollCompletedUI();

            // Next 버튼을 누르면 StatAllocationManager 호출
=======
                $"�� ��°: {thirdRoll}!\n���� �ֻ��� �հ�: {firstRoll} + {secondRoll} + {thirdRoll} = {totalDiceSum}����Ʈ ȹ��!";
            DiceRoller.Instance.SetRollCompletedUI();

            // Next ��ư�� ������ StatAllocationManager ȣ��
>>>>>>> Stashed changes
            DiceRoller.Instance.onNextAction = StartStatAllocation;
        }
    }

    private void StartStatAllocation()
    {
<<<<<<< Updated upstream
        // 주사위 패널 숨기기
=======
        // �ֻ��� �г� �����
>>>>>>> Stashed changes
        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.HideDicePanel();
        }

<<<<<<< Updated upstream
        // StatAllocationManager를 호출하여 스탯 분배 시작
        if (StatAllocationManager.Instance != null)
        {
            // 분배가 끝난 후 FinalPracticeDialogue 함수를 실행하도록 콜백 전달
=======
        // StatAllocationManager�� ȣ���Ͽ� ���� �й� ����
        if (StatAllocationManager.Instance != null)
        {
            // �й谡 ���� �� FinalPracticeDialogue �Լ��� �����ϵ��� �ݹ� ����
>>>>>>> Stashed changes
            StatAllocationManager.Instance.StartAllocation(totalDiceSum, FinalPracticeDialogue);
        }
        else
        {
<<<<<<< Updated upstream
            Debug.LogError("StatAllocationManager 인스턴스를 찾을 수 없습니다! 최종 대화로 바로 이동합니다.");
=======
            Debug.LogError("StatAllocationManager �ν��Ͻ��� ã�� �� �����ϴ�! ���� ��ȭ�� �ٷ� �̵��մϴ�.");
>>>>>>> Stashed changes
            FinalPracticeDialogue();
        }
    }

    private void FinalPracticeDialogue()
    {
<<<<<<< Updated upstream
        // 주사위 굴림 결과를 초기화
=======
        // �ֻ��� ���� ����� �ʱ�ȭ
>>>>>>> Stashed changes
        firstRoll = secondRoll = thirdRoll = 0;

        string[] dialogue = new string[]
        {
<<<<<<< Updated upstream
            "성공적인 연습이었습니다!",
            "획득한 포인트는 캐릭터 스탯에 영구적으로 적용되었습니다."
=======
            "�������� �����̾����ϴ�!",
            "ȹ���� ����Ʈ�� ĳ���� ���ȿ� ���������� ����Ǿ����ϴ�."
>>>>>>> Stashed changes
        };

        if (GMManager.Instance != null)
        {
            GMManager.Instance.StartDialogue(dialogue, () => {
                if (Config.Resource.StageData.CurrentStage == Constant.Stage.TITLE_STAGE)
                {
                    Config.Resource.StageData.CurrentStage = Constant.Stage.STAGE_ONE;
                }
                UnityEngine.SceneManagement.SceneManager.LoadScene("StageScene");
                });
        }
    }
}
