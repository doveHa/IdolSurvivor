namespace Script
{
    public static class Config
    {
        public static string Gender { private get; set; } = "Female";

        public static string CharacterDataPath
        {
            get => $"ScriptableObjects/Character/{Gender}/";
        }

        public static string PlayerCharacterDataPath
        {
            get => $"ScriptableObjects/Character/{Gender}/Player";
        }
    }
}