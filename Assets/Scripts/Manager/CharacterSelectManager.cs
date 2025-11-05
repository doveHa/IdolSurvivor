using System;
using Mono.Cecil;
using Script.Characters;
using Script.DataDefinition.Enum;
using Script.DataDefinition.ScriptableObjects;
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
            Player = new Character(ResourceManager.Load<CharacterData>(Config.PlayerCharacterDataPath));
            settingCount = 0;
        }

        public void SetMode(bool isFemale)
        {
            IsFemale = isFemale;
            if (IsFemale)
            {
                Config.Gender = "Female";
            }
            else
            {
                Config.Gender = "Male";
            }
        }

        public void SetStat(string statType, int statValue)
        {
            StatType type = (StatType)Enum.Parse(typeof(StatType), statType);
            Player.Stat.AddStat(type, statValue);
            settingCount++;

            Debug.Log(settingCount);
            if (settingCount >= Constant.Stat.NUMBER_OF_TYPES)
            {
                nextSceneButton.SetActive(true);
            }
        }
    }
}