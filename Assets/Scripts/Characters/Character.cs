using System;
using Script.DataDefinition.ScriptableObjects;
using Unity.VisualScripting;

namespace Script.Characters
{
    public class Character : IComparable<Character>
    {
        public CharacterStats Stat { get; private set; }
        public int VoteCount { get; private set; }
        public float VoteRatio { get; set; }
        public string PositionName { get; set; }

        public int Rank { get; set; }
        public CharacterData Data { get; private set; }
        public bool IsPlayer { get; private set; }

        public Character(CharacterData data, bool isPlayer = false)
        {
            Stat = new CharacterStats();
            VoteCount = 0;
            Rank = 0;
            Data = data;
            IsPlayer = isPlayer;
        }

        public void AddVote(int voteCount)
        {
            VoteCount += voteCount;
        }

        public int CompareTo(Character other)
        {
            int cmp = other.VoteCount.CompareTo(VoteCount);
            if (cmp == 0)
            {
                cmp = Data.name.CompareTo(other.Data.name);
            }

            return cmp;
        }
    }
}