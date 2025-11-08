using UnityEngine;
using TMPro;
using Script.DataDefinition.Enum;

[System.Serializable]
public struct TraineeData
{
    public int rank;
    public string agency;
    public string traineeName;
    public int votes;
    public Sprite characterImage;
    public bool isPlayer;
    public StatType position;
}