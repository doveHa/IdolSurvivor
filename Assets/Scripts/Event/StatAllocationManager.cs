using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
using Script.DataDefinition.Enum;

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

        // 모든 할당 값을 0으로 초기화
        foreach (ref var entry in allocationEntries.AsSpan())
        {
            entry.currentAllocation = 0;
            entry.allocationValueText.text = "+0";
            // 기존 스탯 값도 0으로 임시 설정 (나중에 로드 기능 구현)
            entry.currentValueText.text = "0";
        }

        UpdateButtonsInteractable();
    }

    // -----------------------------------------------------------
    // 내부 로직: 포인트 할당/제거
    // -----------------------------------------------------------

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
        // 1. 최종 결과 콘솔에 출력 (추후 스탯 적용 로직으로 대체)
        Debug.Log("--- 스탯 분배 결과 ---");

        foreach (var entry in allocationEntries)
        {
            if (entry.currentAllocation > 0)
            {
                Debug.Log($"{entry.statType}에 {entry.currentAllocation} 포인트 적용 예정.");
                // PlayerManager.Instance.AddStat(entry.statType, entry.currentAllocation); // 실제 스탯 적용
            }
        }

        // 2. UI 닫기 및 콜백 실행
        allocationPanel.SetActive(false);
        onApplyCallback?.Invoke();
        onApplyCallback = null;
    }
}
