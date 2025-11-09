using System.Collections.Generic;
using Script.Stage.Event;
using Script.Stage.Event.TitleStage;
using UnityEngine;

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
                    return $"ScriptableObjects/Player/{Gender}/Player";
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

        public static class Event
        {
            public static int EventCount { get; set; }

            private static List<StageEvent> events = new List<StageEvent>();

            public static void EventSetting()
            {
                switch (Config.Resource.StageData.CurrentStage)
                {
                    case Constant.Stage.TITLE_STAGE:
                        events.Add(new MinusVoteEvent().Initialize());
                        events.Add(new PlusVoteEvent().Initialize());
                        events.Add(new PlusVoteEvent().Initialize());
                        break;
                }
            }

            public static StageEvent GetEvent()
            {
                if (events.Count == 0)
                {
                    return null;
                }

                int randomIndex = Random.Range(0, events.Count);
                StageEvent randomEvent = events[randomIndex];
                events.RemoveAt(randomIndex);
                return randomEvent;
            }
        }


        public static class Team
        {
            public static int TeamCount { get; set; } = 4;
            public static int AllCharacterCount { get; set; } = 12;
        }
    }
}