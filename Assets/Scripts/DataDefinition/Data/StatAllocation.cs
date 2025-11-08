using Script.DataDefinition.Enum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StatAllocation
{
    public StatType statType; // 어떤 스탯인지 구분 (Sing, Dance, Charm 등)
    public TMPro.TextMeshProUGUI currentValueText; // 기존 스탯 값 표시 (현재는 0으로 가정)
    public TMPro.TextMeshProUGUI allocationValueText; // 추가된 분배 값 (+1, -1)
    public Button addButton;
    public Button subtractButton;

    // 이 항목에 현재 할당된 값 (스크립트 내부에서 관리)
    [HideInInspector] public int currentAllocation;
}
