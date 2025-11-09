using System.Collections.Generic;
using System.Linq;
using Script.Characters;
using Script.DataDefinition.ScriptableObjects;
using UnityEngine;

namespace Script.Manager
{
    public class AllCharacterManager : ManagerBase<AllCharacterManager>
    {
        public Character Player { get; private set; }
        public List<Character> OtherCharacters { get; private set; }
        public List<Character> Ranks { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Player = new Character(
                ResourceManager.Load<CharacterData>(Config.Resource.CharacterData.PlayerCharacterDataPath()));
            OtherCharacters = new List<Character>();
            Ranks = new List<Character>();
        }

        public void GenerateAllCharacters()
        {
            CharacterData[] data =
                ResourceManager.LoadAll<CharacterData>(Config.Resource.CharacterData.CharacterDataPath());
            foreach (CharacterData characterData in data)
            {
                Character character = new Character(characterData);
                character.Stat.SetRandomStat();
                OtherCharacters.Add(character);
            }

            Shuffle();
        }

        public void CalculateRank()
        {
            SortedSet<Character> set = new SortedSet<Character>();

            int rank = 1;
            foreach (KeyValuePair<int, List<Character>> kvp in Ranking().Reverse())
            {
                List<Character> list = kvp.Value;
                foreach (Character character in list)
                {
                    character.Rank = rank;
                    set.Add(character);
                }

                rank += list.Count;
            }

            Ranks = set.ToList();
        }

        public void DropOut()
        {
            Ranks.RemoveAt(Ranks.Count - 1);
            Ranks.RemoveAt(Ranks.Count - 2);
            Ranks.RemoveAt(Ranks.Count - 3);
        }
        
        private SortedDictionary<int, List<Character>> Ranking()
        {
            SortedDictionary<int, List<Character>> ranks = new SortedDictionary<int, List<Character>>();
            List<Character> allCharacters = new List<Character>(OtherCharacters);
            allCharacters.Add(Player);

            foreach (Character character in allCharacters)
            {
                if (!ranks.TryGetValue(character.VoteCount, out List<Character> list))
                {
                    list = new List<Character>();
                    ranks.Add(character.VoteCount, list);
                }

                list.Add(character);
            }

            return ranks;
        }

        private void Shuffle()
        {
            int count = Constant.Character.ALL_MEMBER - 1;

            while (count > 0)
            {
                count--;
                int random = Random.Range(0, OtherCharacters.Count);
                (OtherCharacters[count], OtherCharacters[random]) = (OtherCharacters[random], OtherCharacters[count]);
            }
        }
    }
}