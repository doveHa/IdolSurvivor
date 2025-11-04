namespace Script.Characters
{
    public class Character
    {
        public CharacterStats Stat { get; private set; }
        public int VoteCount { get; private set; }
        public int Rank { get; private set; }
        public string Name { get; private set; }

        public Character(string name)
        {
            Stat = new CharacterStats();
            VoteCount = 0;
            Rank = 0;
            Name = name;
        }

        public Character(CharacterStats stat, int voteCount, string name)
        {
            Stat = stat;
            VoteCount = voteCount;
            Rank = 0;
            Name = name;
        }
    }
}