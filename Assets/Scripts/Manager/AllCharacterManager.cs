using System.Collections.Generic;
using Script.Characters;
using Script.DataDefinition.ScriptableObjects;

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
        }
    }
}