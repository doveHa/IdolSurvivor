using System;
using System.Collections.Generic;
using System.Linq;
using Script.DataDefinition.Data;
using Script.DataDefinition.Enum;
using UnityEngine;

namespace Script.Characters
{
    public class Team
    {
        private static Dictionary<StatType, float> statCoffs;

        private Character leader;
        public List<Character> Teams { get; private set; }

        public GameObject teammateSlot;

        private TeamColor teamColor;

        public Team(Character leader)
        {
            statCoffs = new Dictionary<StatType, float>();
            statCoffs.Add(StatType.Sing, 1);
            statCoffs.Add(StatType.Dance, 1);
            statCoffs.Add(StatType.Appearance, 1);
            statCoffs.Add(StatType.Charm, 1);
            this.leader = leader;
            Teams = new List<Character>();
            Teams.Add(leader);
        }

        public void AddTeamMate(Character teammate)
        {
            Teams.Add(teammate);
        }

        public void SetSlot(GameObject slot)
        {
            teammateSlot = slot;
        }

        public TeamColor GetTeamColor(CharacterStats totalStat)
        {
            Stat maxStat = new Stat(StatType.Sing, -1);
            int sumValue = 0;
            foreach (Stat stat in totalStat.ToStatArray())
            {
                if (stat.Value > maxStat.Value)
                {
                    maxStat = stat;
                }

                sumValue += stat.Value;
            }

            sumValue -= maxStat.Value;
            sumValue /= Constant.Team.MAX_MEMBER - 1;

            if (IsOneManTeam())
            {
                return TeamColor.OneMan;
            }
            else if (maxStat.Value >= sumValue * Constant.Harmony.TEAM_COLOR_COFF)
            {
                return StatTypeToTeamColor(maxStat.StatType);
            }
            else
            {
                return TeamColor.None;
            }
        }

        public CharacterStats TotalStat()
        {
            CharacterStats totalStat = new CharacterStats();

            foreach (Character stat in Teams)
            {
                if (stat != null)
                {
                    totalStat.AddStat(stat.Stat);
                }
            }

            return totalStat;
        }

        public CharacterStats PredictionAddStat(CharacterStats stat)
        {
            CharacterStats tempStat = new CharacterStats(TotalStat());
            tempStat.AddStat(stat);
            return tempStat;
        }

        public float CalculateTotalPerformance()
        {
            CharacterStats totalStat = TotalStat();

            float performance = totalStat.Sing.Value * statCoffs[StatType.Sing]
                                + totalStat.Dance.Value * statCoffs[StatType.Dance]
                                + totalStat.Appearance.Value * statCoffs[StatType.Appearance]
                                + totalStat.Charm.Value * statCoffs[StatType.Charm];
            return performance * HarmonyCoff();
        }

        public void SetStatCoff(StatType statType, float value)
        {
            statCoffs[statType] = value;
        }

        private bool IsOneManTeam()
        {
            int leaderSumStat = leader.Stat.SumStat();
            int teamSumStat = 0;
            foreach (Character stat in Teams)
            {
                teamSumStat += stat.Stat.SumStat();
            }

            teamSumStat -= leaderSumStat;

            return leaderSumStat > teamSumStat * Constant.Harmony.TEAM_COLOR_COFF;
        }

        private TeamColor StatTypeToTeamColor(StatType statType)
        {
            switch (statType)
            {
                case StatType.Sing:
                    return TeamColor.Fresh;
                case StatType.Dance:
                    return TeamColor.Sexy;
                case StatType.Charm:
                    return TeamColor.Elegance;
                case StatType.Appearance:
                    return TeamColor.Visual;
                default:
                    return TeamColor.None;
            }
        }

        private float HarmonyCoff()
        {
            int[] totalStats = TotalStat().ToValueArray();

            double average = totalStats.Average();
            double variance = totalStats.Select(s => (s - average) * (s - average)).Average();
            double stdDev = Math.Sqrt(variance);
            double normalize = stdDev / average;
            return (float)(Constant.Harmony.MAX_COFF -
                           (Constant.Harmony.MAX_COFF - Constant.Harmony.MIN_COFF) * normalize);
        }
    }
}