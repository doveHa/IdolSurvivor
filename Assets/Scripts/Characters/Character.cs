namespace Script.Characters
{
    public class Character
    {
        public CharacterStats Stat { get; private set; }
        public int VoteCount { get; private set; }
        public int Rank { get; private set; }
        public string Name { get; private set; }

        public Character()
        {
            Stat = new CharacterStats();
            VoteCount = 0;
            Rank = 0;
        }

        public void SetName(string name)
        {
            Name = name;
        }
    }
}