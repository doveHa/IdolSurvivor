namespace Script
{
    public static class Constant
    {
        public static class InitialVoteCount
        {
            public const int GRADE_A = 50;
            public const int GRADE_B = 30;
            public const int GRADE_C = 10;
        }

        public static class Stat
        {
            public const int NUMBER_OF_TYPES = 4;
        }

        public static class Character
        {
            public const int ALL_MEMBER = 12;
        }

        public static class Team
        {
            public const int MAX_COUNT = 4;
            public const int MAX_MEMBER = 3;
        }

        public static class Harmony
        {
            public const float MAX_COFF = 1.5f;
            public const float MIN_COFF = 0.5f;
            public const float TEAM_COLOR_COFF = 1.2f;
        }

        public static class Stage
        {
            public const int PROGRESS_TIME = 10;
            public const string TITLE_STAGE = "TitleStage";
            public const string STAGE_ONE = "StageOne";
            public const string STAGE_TWO = "StageTwo";
            public const string FINAL_STAGE = "FinalStage";
        }

        public static class Scene
        {
            public const string GRADING_SCENE = "GradingScene";
            public const string STAGE_SCENE = "StageScene";
            public const string TEAM_BUILDING = "TeamBuildingScene";
            public const string SELECT_SONG = "BeforeShow";
            public const string RANKING_ANNOUNCE = "RankingAnnouncement";
        }

        public static class UIAnimationNumber
        {
            public const int PROGRESS_BAR_KNOB_IDLE = 0;
        }
    }
}