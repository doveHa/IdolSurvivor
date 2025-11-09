using Script.DataDefinition.Enum;
using UnityEngine;

namespace Script.DataDefinition.ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewStage", menuName = "Stage")]
    public class StageData : ScriptableObject
    {
        public Sprite backGround;

        public string title;
        
        //public Sound sound;
        public StatType plusStat, minusStat;
    }
}