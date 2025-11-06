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
                public static string NextStage { private get; set; }


                public static string NextStageDataPath()
                {
                    return $"ScriptableObjects/Stage/{NextStage}/";
                }
            }
        }
    }
}