using Script.DataDefinition.Enum;
using UnityEngine;

public enum ConceptType
{
    Title,
    Sexy,
    Fresh,
    Dance,
    Cute,
    None
}

[System.Serializable]
public class SongData
{
    public string songName;
    public ConceptType concept;

    public StatType increaseStat;
    public StatType decreaseStat;
}
