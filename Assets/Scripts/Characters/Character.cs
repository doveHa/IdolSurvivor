using Script.DataDefinition.ScriptableObjects;

namespace Script.Characters
{
    public class Character
    {
        public CharacterStats Stat { get; private set; }
        public int VoteCount { get; private set; }
        public int Rank { get; private set; }
        public CharacterData Data { get; private set; }

        public Character(CharacterData data)
        {
            Stat = new CharacterStats();
            VoteCount = 0;
            Rank = 0;
            Data = data;
        }

        public void AddVote(int voteCount)
        {
            VoteCount += voteCount;
        }
    }
}