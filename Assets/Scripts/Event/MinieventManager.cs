using Script; // DiceRoller ���ӽ����̽�
using Script.DataDefinition.Enum;
using Script.Manager;
using System;
using System.Linq;
using TMPro; // TMPro ���ӽ����̽�
using UnityEngine;
using UnityEngine.UI;
using Script.Characters;
using Script.TeamBuilding;
using UnityEngine.SceneManagement;

public class MinieventManager : MonoBehaviour
{
    public enum EventType
    {
        PR_1Min,
        Practice,
        StreetPerformance,
        Entertainment
    }

    [Header("Event Settings")] public EventType currentEvent = EventType.PR_1Min;

    [Header("Street Performance UI")] public GameObject statSelectionPanel; // Inspector���� �Ҵ� �ʿ�

    public StatButton[] statButtons;

    // ���� ���� ���� �ʵ�
    private int[] currentRolls; // ���� �̺�Ʈ�� ��� �ֻ��� ����� ����
    private int requiredRolls = 0; // �ʿ��� �� ���� Ƚ��
    private int currentRollIndex = 0; // ���� ���� Ƚ�� �ε���
    private Action<int[]> onRollsCompleteCallback; // ��� ���� �Ϸ� �� ����� �ݹ�

    // �̺�Ʈ ��� �ʵ�
    private int firstRoll = 0;
    private int finalVotesResult = 0;
    private int practicePoints = 0;
    private StatType selectedStatForStreet;

    // ��Ÿ� ���� ���� ��ȭ�� ���
    private const float StreetSuccessMultiplier = 3.0f;
    private const float StreetFailureReduction = 0.20f; // 20% ����

    void Start()
    {
        currentEvent = GetRandomEventType();
        Debug.Log($"���õ� �̺�Ʈ: {currentEvent}");
        StartCurrentEvent();
    }

    private void StartCurrentEvent()
    {
        // StartCurrentEvent�� ȣ��� ������ �ش� �̺�Ʈ�� ���۵ǵ��� ó��
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
        // ��� StatType�� ���� �ʿ��� ������ �߰��ϼ���.
        switch (type)
        {
            case StatType.Sing:
                return "�뷡";
            case StatType.Dance:
                return "��";
            case StatType.Charm:
                return "�ŷ�";
            case StatType.Appearance:
                return "�ܸ�";
            default:
                return type.ToString();
        }
    }

    public EventType GetRandomEventType()
    {
        EventType[] allEvents = (EventType[])Enum.GetValues(typeof(EventType));

        if (allEvents.Length == 0)
        {
            Debug.LogError("EventType Enum�� ���ǵ� �̺�Ʈ�� �����ϴ�. PR_1Min�� �⺻������ ��ȯ�մϴ�.");
            return EventType.PR_1Min;
        }

        int randomIndex = UnityEngine.Random.Range(0, allEvents.Length);

        return allEvents[randomIndex];
    }

    // =================================================================
    // ���� ��Ƽ �� �ý��� (ȿ��ȭ�� �ٽ�)
    // =================================================================

    /// <summary>
    /// N���� �ֻ����� ������ ����� �迭�� ������ ��, ���� �ݹ��� �����ϴ� ���� �Լ��Դϴ�.
    /// </summary>
    private void StartMultiRoll(int numRolls, Action<int[]> finalCallback)
    {
        requiredRolls = numRolls;
        currentRollIndex = 0;
        currentRolls = new int[numRolls];
        onRollsCompleteCallback = finalCallback;

        // ù ��° ������ �����ϵ��� UI ���� �� ������ ����
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
                // ������ ���� �� NextDiceOrFinalize�� ȣ���ϵ��� ��û
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
            // ���� �ֻ����� ���� ���: ���� ���� �غ� (Next ��ư���� ��ȯ)
            DiceRoller.Instance.resultText.text =
                $"���� #{currentRollIndex} ���: {rollResult}!\n���� ������ �غ��ϼ���.";

            DiceRoller.Instance.SetRollCompletedUI(); // Roll -> Next ��ư ��ȯ

            DiceRoller.Instance.onNextAction = () =>
            {
                // Next ��ư Ŭ�� �� ���� �ֻ��� ���� ��ư �ؽ�Ʈ ���� �� ������ ���Ҵ�
                ShowDiceAndSetRollListener();
                DiceRoller.Instance.rollButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
                    $"Roll ({currentRollIndex + 1}/{requiredRolls})";
            };
        }
        else
        {
            // ��� ���� �Ϸ�: ���� ó�� ����
            DiceRoller.Instance.resultText.text = $"��� ���� �Ϸ�! ���� ����� Ȯ���ϼ���.";
            DiceRoller.Instance.SetRollCompletedUI(); // Roll -> Next ��ư ��ȯ

            // Next ��ư�� ���� ó�� �ݹ� ����
            DiceRoller.Instance.onNextAction = () =>
            {
                //DiceRoller.Instance.HideDicePanel();
                //onRollsCompleteCallback.Invoke(currentRolls); // ���� ó�� �Լ��� ��� �迭 ����
                onRollsCompleteCallback.Invoke(currentRolls);
            };
        }
    }

    private void NextScene()
    {
        Destroy(TeamBuildingManager.Manager.gameObject);
        SceneManager.LoadScene("TeamBuildingScene");
        Config.Team.TeamCount = 3; // 4 -> 3
        Config.Team.AllCharacterCount = 9; // 12 -> 9
        //MiniEvnet1�� ��ü ���� �÷ο쿡�� �ѹ��� ȣ��ǹǷ� �� �ϵ��ڵ� �߽��ϴ�...
    }


    // =================================================================
    // 1�� PR �̺�Ʈ (Roll 2ȸ)
    // =================================================================

    private void StartOneMinPR()
    {
        string[] dialogue = { "�̴� �̺�Ʈ '1�� PR'�� ���� ���� ȯ���մϴ�!", "�ֻ����� �� �� ���� ��ǥ���� �����մϴ�." };
        if (GMManager.Instance != null)
        {
            // StartMultiRoll�� ����Ͽ� RollFirstPRDice, RollSecondPRDice �Լ��� ������
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

        // ��ǥ�� ��� ����
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

        // ��ǥ�� ���� ���⿡
        AllCharacterManager.Manager.Player.AddVote(finalVotesResult);

        FinalPRDialogue();
    }

    private void FinalPRDialogue()
    {
        string[] dialogue = { $"1�� PR�� �������ϴ�. ��ǥ�� {finalVotesResult}ǥ�� ȹ���Ͽ����ϴ�.", "���� �̺�Ʈ�� �̵��մϴ�." };
        if (GMManager.Instance != null)
        {
            GMManager.Instance.StartDialogue(dialogue, () => NextScene());
        }
    }


    // =================================================================
    // ���� �̺�Ʈ (Roll 2ȸ -> Stat �й�)
    // =================================================================

    private void StartPractice()
    {
        string[] dialogue = { "���� �̺�Ʈ�� �����մϴ�. �ֻ��� �ո�ŭ ���� ����Ʈ�� ��� �й��մϴ�." };
        if (GMManager.Instance != null)
        {
            GMManager.Instance.StartDialogue(dialogue,
                () => StartMultiRoll(2, ProcessPracticeResult));
        }
    }

    private void ProcessPracticeResult(int[] rolls)
    {
        practicePoints = rolls[0] + rolls[1];

        // Next ��ư�� ������ StatAllocationManager ȣ��
        StartPracticeAllocation();
    }

    private void StartPracticeAllocation()
    {
        if (DiceRoller.Instance != null) DiceRoller.Instance.HideDicePanel();

        if (StatAllocationManager.Instance != null)
        {
            // �й谡 ���� �� FinalPracticeDialogue �Լ��� �����ϵ��� �ݹ� ����
            StatAllocationManager.Instance.StartAllocation(practicePoints, FinalPracticeDialogue);
        }
        else
        {
            Debug.LogError("StatAllocationManager �ν��Ͻ��� ã�� �� �����ϴ�! ���� ��ȭ�� �ٷ� �̵��մϴ�.");
            FinalPracticeDialogue();
        }
    }

    private void FinalPracticeDialogue()
    {
        string[] dialogue =
            { $"������ �������ϴ�. �� {practicePoints} ����Ʈ�� ���ȿ� �й��߽��ϴ�.", "���� �̺�Ʈ�� �̵��մϴ�." };
        if (GMManager.Instance != null)
        {
            GMManager.Instance.StartDialogue(dialogue, () => NextScene());
        }
    }


    // =================================================================
    // ��Ÿ� ���� �̺�Ʈ (���� ���� �� Roll 1ȸ)
    // =================================================================

    private void StartStreetPerformance()
    {
        string[] dialogue =
        {
            "���α׷��� ȫ���ϱ� ���� ��Ÿ� ���� �̺�Ʈ�� �����մϴ�!",
            "� ���������� ��Ÿ� ���� �̺�Ʈ�� �����ðڽ��ϱ�?"
        };

        if (GMManager.Instance != null)
        {
            // ��ȭ�� ���� �� ���� ���� UI Ȱ��ȭ
            GMManager.Instance.StartDialogue(dialogue, ShowStatSelectionUI);
        }
    }

    private void ShowStatSelectionUI()
    {
        if (statSelectionPanel != null)
        {
            // ���� ���� �г��� Ȱ��ȭ�ϰ� GM �г��� �ݽ��ϴ�.
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

                // ���� ������ ���� (�ʼ�)
                btn.onClick.RemoveAllListeners();

                // AddListener�� ����� ���� �Լ��� ���� ����
                btn.onClick.AddListener(() => OnStatSelectedByCode(statBtn.statType));
            }
        }
    }

    public void OnStatSelectedByCode(StatType type)
    {
        // 2. ���õ� ���� ����
        selectedStatForStreet = type;

        if (GMManager.Instance != null)
        {
            string translatedStat = GetKoreanStatName(type);

            // 3. GM �гο� Ȯ�� �޽��� ǥ��
            GMManager.Instance.gmText.text =
                $"{translatedStat} ������ �����Ͻðڽ��ϱ�?\n" +
                "(���� �� ���� ����, ���� �� ���� ����)";
            GMManager.Instance.gmPanel.SetActive(true);

            // 4. Next ��ư�� �ֻ��� ���� ���� �Լ� ����
            GMManager.Instance.NextBtn.onClick.RemoveAllListeners();
            GMManager.Instance.NextBtn.onClick.AddListener(RollStreetPerformanceDice);
        }
    }


    private void RollStreetPerformanceDice()
    {
        // 1. ���� ���� UI �� GM Ȯ�� �г� ��Ȱ��ȭ
        if (statSelectionPanel != null)
            statSelectionPanel.SetActive(false);
        if (GMManager.Instance != null)
            GMManager.Instance.gmPanel.SetActive(false);

        if (DiceRoller.Instance != null)
        {
            // 2. �ֻ��� �г� ����
            DiceRoller.Instance.ShowDicePanel();

            Button rollBtn = DiceRoller.Instance.rollButton;
            if (rollBtn != null)
            {
                // 3. Roll ��ư ������ ����: ���� �Ϸ� �� ProcessStreetResult ����
                rollBtn.onClick.RemoveAllListeners();
                rollBtn.onClick.AddListener(() =>
                {
                    // ���� ���� �ٽ�: RollDiceWithCallback�� ProcessStreetResult ����
                    DiceRoller.Instance.RollDiceWithCallback(ProcessStreetResultManual);
                    rollBtn.interactable = false;
                });

                // UI �ؽ�Ʈ ����
                rollBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Roll";
                rollBtn.interactable = true;
            }
        }
    }

    private void ProcessStreetResultManual(int roll)
    {
        // ProcessStreetResult ������ ���� �����մϴ�.
        string translatedStat = GetKoreanStatName(selectedStatForStreet);

        // 1. ���� �� �������� (�ӽ�)
        int baseValue = GetPlayerStatValue(selectedStatForStreet);

        DiceCheckResult check = JudgeStreetRoll(roll);
        string resultMessage;
        int change = 0;

        // 2. ����/���� ���� �� ���� ���� ���
        if (check == DiceCheckResult.Success)
        {
            change = (int)(baseValue * StreetSuccessMultiplier) -
                     baseValue; // ���� ������ ��� (��: 10 -> 30, change = 20)

            resultMessage =
                $"[����: {roll}] ����!\n" +
                $"{translatedStat} ������ {StreetSuccessMultiplier}�� �����Ͽ� �� {baseValue + change}�� �Ǿ����ϴ�! (+{change})";

            // ���� ����
            ApplyStatChange(selectedStatForStreet, change);
        }
        else
        {
            int reductionAmount = (int)(baseValue * StreetFailureReduction); // 10 * 0.20f = 2
            change = -reductionAmount;

            resultMessage =
                $"[����: {roll}] ����!\n" +
                $"������ 20% �����Ͽ� {reductionAmount}��ŭ �پ������ϴ�.";

            ApplyStatChange(selectedStatForStreet, change);
        }

        // 3. UI ������Ʈ �� Next ��ư ����
        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.resultText.text = resultMessage;
            DiceRoller.Instance.SetRollCompletedUI();

            // 4. Next ��ư Ŭ�� �� FinalStreetDialogue ����
            DiceRoller.Instance.onNextAction = FinalStreetDialogue;
        }
    }

    private void FinalStreetDialogue()
    {
        Debug.Log("FinalStreetDialogue ȣ���");

        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.HideDicePanel();
        }

        string[] dialogue = new string[]
        {
            "��Ÿ� ���� �̺�Ʈ�� �������ϴ�.",
            "���� �̺�Ʈ�� �̵��մϴ�."
        };

        if (GMManager.Instance != null)
        {
            GMManager.Instance.StartDialogue(dialogue, FinalActionOfStreetEvent);
        }
    }

    private void FinalActionOfStreetEvent()
    {
        Debug.Log("��Ÿ� ���� �̺�Ʈ ����");
        NextScene();
        // ���⿡ �� ��ȯ �Ǵ� ���� �̺�Ʈ ���� ������ �ֽ��ϴ�.
    }

    // ��Ÿ� ���� ���� ���� (1~3: ����, 4~6: ����)
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
            "PPL ���� �ڳʿ��� Ȱ���߽��ϴ�.",
            $"��ǥ�� {guaranteedVotes}ǥ�� ������ϴ�.\n���� �ܰ�� �̵��մϴ�."
        };

        if (GMManager.Instance != null)
        {
            GMManager.Instance.StartDialogue(dialogue, () =>
            {
                // ��ǥ�� �߰�
                AllCharacterManager.Manager.Player.AddVote(guaranteedVotes);
                NextScene();
                Debug.Log("���� �̺�Ʈ ��. ���� ������ ��ȯ");
            });
        }
    }

    private int GetPlayerStatValue(StatType type)
    {
        if (AllCharacterManager.Manager != null && AllCharacterManager.Manager.Player != null)
        {
            CharacterStats playerStats = AllCharacterManager.Manager.Player.Stat;
            switch (type)
            {
                case StatType.Sing: return playerStats.Sing.Value;
                case StatType.Dance: return playerStats.Dance.Value;
                case StatType.Charm: return playerStats.Charm.Value;
                case StatType.Appearance: return playerStats.Appearance.Value;
            }
        }

        return 10; // ������� ���� �⺻�� ��ȯ 
    }

    private void ApplyStatChange(StatType type, int amount)
    {
        if (AllCharacterManager.Manager != null && AllCharacterManager.Manager.Player != null)
        {
            var playerStats = AllCharacterManager.Manager.Player.Stat;

            // CharacterStats�� AddStatValue �Ǵ� NewStat�� Ȱ���Ͽ� ������ ������Ʈ�մϴ�.
            // AddStatValue(StatType, int) �Լ��� CharacterStats�� �ִٰ� �����մϴ�.
            playerStats.AddStatValue(type, amount);

            Debug.Log($"[StreetPerformance] {type} ���ȿ� {amount} �ݿ� �Ϸ�.");
            Debug.Log($"[StreetPerformance] ���� ����: {playerStats.ToString()}");
        }
        else
        {
            Debug.LogError("AllCharacterManager�� Player ���� ��ü�� ã�� �� ���� ���� ������ ������ �� �����ϴ�.");
        }
    }
}