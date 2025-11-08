namespace Script
{
    public static class Config
    {
        public static class Resource
        {
            public static class CharacterData
            {
                public static string Gender { private get; set; } = "Female";

                public static string CharacterDataPath()
                {
                    return $"ScriptableObjects/Character/{Gender}/";
                }

                public static string PlayerCharacterDataPath()
                {
                    return $"ScriptableObjects/Character/{Gender}/Player";
                }
            }

            public static class StageData
            {
                public static string CurrentStage { get; set; }

                public static string CurrentStageDataPath()
                {
                    return $"ScriptableObjects/Stage/{CurrentStage}";
                }
            }
        }
    }
}