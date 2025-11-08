using System.Collections.Generic;
using Script.Stage.Event.TitleStage;
using UnityEngine;

namespace Script.Stage.Event
{
    public static class EventConfig
    {
        public static int EventCount { get; set; }

        private static List<StageEvent> events = new List<StageEvent>();

        public static void EventSetting()
        {
            events.Clear();
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
}