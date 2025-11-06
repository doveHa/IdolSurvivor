using System.Collections.Generic;
using Script.Characters;
using Script.DataDefinition.ScriptableObjects;
using UnityEngine;

namespace Script.Manager
{
    public class AllCharacterManager : ManagerBase<AllCharacterManager>
    {
        public List<Character> AllCharacters { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            AllCharacters = new List<Character>();
        }

        public void GenerateAllCharacters()
        {
            CharacterData[] data = ResourceManager.LoadAll<CharacterData>(Config.CharacterDataPath);
            foreach (CharacterData character in data)
            {
                if (character.name.Equals(CharacterSelectManager.Manager.Player.Data.name))
                {
                    AllCharacters.Add(CharacterSelectManager.Manager.Player);
                }
                else
                {
                    AllCharacters.Add(new Character(character));
                }
            }

            Shuffle();
        }

        private void Shuffle()
        {
            int count = Constant.Character.ALL_MEMBER;

            while (count > 0)
            {
                count--;
                int random = Random.Range(0, AllCharacters.Count);
                (AllCharacters[count], AllCharacters[random]) = (AllCharacters[random], AllCharacters[count]);
            }
        }
    }
}