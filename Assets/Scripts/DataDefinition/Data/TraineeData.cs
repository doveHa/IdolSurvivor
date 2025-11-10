using Script.Characters;
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

    public TraineeData(Character character)
    {
        rank = character.Rank;
        agency = "";
        traineeName = character.Data.name;
        votes = character.VoteCount;
        characterImage = character.Data.standingImage;
        //isPlayer = false;
        isPlayer = character.IsPlayer;
        position = StatType.None;
    }
}