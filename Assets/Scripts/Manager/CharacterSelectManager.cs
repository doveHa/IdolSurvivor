using System;
using Script.Characters;
using Script.DataDefinition.Enum;
using UnityEngine;

namespace Script.Manager
{
    public class CharacterSelectManager : ManagerBase<CharacterSelectManager>
    {
        [SerializeField] private GameObject nextSceneButton;
        public bool IsFemale { get; private set; }
        public Character Player { get; private set; }

        private int settingCount;

        protected override void Awake()
        {
            base.Awake();
            Player = new Character();
            settingCount = 0;
        }

        public void SetMode(bool isFemale)
        {
            IsFemale = isFemale;
        }

        public void SetStat(string statType, int statValue)
        {
            StatType type = (StatType)Enum.Parse(typeof(StatType), statType);
            Player.Stat.AddStat(type, statValue);
            settingCount++;

            Debug.Log(settingCount);
            if (settingCount >= Constant.NUMBER_OF_STAT_TYPE)
            {
                nextSceneButton.SetActive(true);
            }
        }
    }
}