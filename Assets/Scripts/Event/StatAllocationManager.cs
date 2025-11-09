using Script.Characters;
using Script.DataDefinition.Enum;
using Script.Manager;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;

public class StatAllocationManager : MonoBehaviour
{
    public static StatAllocationManager Instance { get; private set; }

    [Header("UI Components")]
    public GameObject allocationPanel;
    public TMPro.TextMeshProUGUI remainingPointsText;
    public Button applyButton;

    [Header("Stat Allocation Entries")]
    public StatAllocation[] allocationEntries;

    private int totalDiceSum = 0;
    private int remainingPoints = 0;

    private Action onApplyCallback;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        allocationPanel.SetActive(false);
        InitializeUI();
    }

    private void InitializeUI()
    {
        foreach (var entry in allocationEntries)
        {
            entry.addButton.onClick.AddListener(() => AddPoint(entry.statType));
            entry.subtractButton.onClick.AddListener(() => SubtractPoint(entry.statType));

            entry.currentAllocation = 0;
            entry.allocationValueText.text = "+0";
        }

        applyButton.onClick.RemoveAllListeners();
        applyButton.onClick.AddListener(OnApplyClicked);
    }

    // 외부 호출용

    public void StartAllocation(int diceSum, Action onApply)
    {
        totalDiceSum = diceSum;
        remainingPoints = diceSum;
        onApplyCallback = onApply;

        allocationPanel.SetActive(true);
        remainingPointsText.text = remainingPoints.ToString();

        CharacterStats playerStats = null;
        if (AllCharacterManager.Manager != null && AllCharacterManager.Manager.Player != null)
        {
            playerStats = AllCharacterManager.Manager.Player.Stat;
        }

        foreach (var entry in allocationEntries)
        {
            entry.currentAllocation = 0;
            entry.allocationValueText.text = "+0";

            // 기존 스탯 값 출력
            if (playerStats != null)
            {
                int baseStat = GetStatValue(playerStats, entry.statType);
                entry.currentValueText.text = baseStat.ToString();
            }
            else
            {
                entry.currentValueText.text = "Error";
                Debug.LogError("AllCharacterManager나 Player 스탯을 찾을 수 없습니다.");
            }
        }

        UpdateButtonsInteractable();
    }

    // -----------------------------------------------------------
    // 내부 로직: 포인트 할당/제거
    // -----------------------------------------------------------

    private int GetStatValue(CharacterStats stats, StatType type)
    {
        switch (type)
        {
            case StatType.Sing:
                return stats.Sing.Value;
            case StatType.Dance:
                return stats.Dance.Value;
            case StatType.Charm:
                return stats.Charm.Value;
            case StatType.Appearance:
                return stats.Appearance.Value;
            default:
                return 0;
        }
    }

    private void AddPoint(StatType type)
    {
        if (remainingPoints > 0)
        {
            ref var entry = ref FindEntry(type);
            entry.currentAllocation++;
            remainingPoints--;

            UpdateUI(entry.currentAllocation, entry.allocationValueText);
            UpdateRemainingPoints();
            UpdateButtonsInteractable();
        }
    }

    private void SubtractPoint(StatType type)
    {
        ref var entry = ref FindEntry(type);

        if (entry.currentAllocation > 0)
        {
            entry.currentAllocation--;
            remainingPoints++;

            UpdateUI(entry.currentAllocation, entry.allocationValueText);
            UpdateRemainingPoints();
            UpdateButtonsInteractable();
        }
    }

    private void UpdateUI(int allocation, TMPro.TextMeshProUGUI text)
    {
        text.text = (allocation > 0 ? "+" : "") + allocation.ToString();
    }

    private void UpdateRemainingPoints()
    {
        remainingPointsText.text = remainingPoints.ToString();
    }

    private void UpdateButtonsInteractable()
    {
        bool canAdd = remainingPoints > 0;

        foreach (var entry in allocationEntries)
        {
            entry.addButton.interactable = canAdd;
            entry.subtractButton.interactable = entry.currentAllocation > 0;
        }

        applyButton.interactable = remainingPoints == 0;
    }

    // Array.FindAll 대신 Span을 사용하여 성능을 최적화하고 값 복사를 피합니다.
    private ref StatAllocation FindEntry(StatType type)
    {
        for (int i = 0; i < allocationEntries.Length; i++)
        {
            if (allocationEntries[i].statType == type)
            {
                return ref allocationEntries[i];
            }
        }
        // 안전 장치: 찾지 못하면 첫 번째 항목을 반환 (에러 방지용)
        return ref allocationEntries[0];
    }
    // 최종 적용

    private void OnApplyClicked()
    {
        Debug.Log("--- 스탯 분배 결과 ---");

        // 1. 분배된 포인트를 담을 임시 CharacterStats 객체 생성
        CharacterStats distributionStats = new CharacterStats();

        foreach (var entry in allocationEntries)
        {
            if (entry.currentAllocation > 0)
            {
                Debug.Log($"{entry.statType}에 {entry.currentAllocation} 포인트 적용 예정.");

                // 2. 임시 객체에 분배된 포인트 할당
                // NewStat 함수가 StatType과 int 값을 받으므로 사용합니다.
                distributionStats.NewStat(entry.statType, entry.currentAllocation);
            }
        }

        // 3. 플레이어의 기존 스탯에 임시 객체 (분배 포인트)를 합산
        var playerStats = AllCharacterManager.Manager.Player.Stat;
        playerStats.AddStat(distributionStats);

        Debug.Log("스탯 분배 완료. 플레이어 최종 스탯: " + playerStats.ToString());

        // UI 닫기 및 콜백 실행
        allocationPanel.SetActive(false);
        onApplyCallback?.Invoke();
        onApplyCallback = null;
    }
}
