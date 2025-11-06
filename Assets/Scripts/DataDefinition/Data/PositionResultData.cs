using UnityEngine;
using Script.DataDefinition.Enum;

[System.Serializable]
public struct PositionResultData
{
    // 포지션 결정 결과 데이터
    public DiceCheckResult checkResult; // 대성공, 성공, 실패
    public string positionName;         // 포지션 이름 (예: 센터, 메모)
    public float voteRatio;             // 득표수 비율 (예: 0.5f)
}

[System.Serializable]
public struct LineDistributionData
{
    // 분량 결정 결과 데이터
    public DiceCheckResult checkResult; // 대성공, 중간, 실패
    public string distributionLabel;    // 분량 라벨 (예: 많음, 중간)
    public int eventCount;              // 이벤트 수 (예: 5)
}