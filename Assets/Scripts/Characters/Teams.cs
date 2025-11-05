using System;
using System.Collections.Generic;
using System.Linq;
using Script.DataDefinition.Data;
using Script.DataDefinition.Enum;

namespace Script.Characters
{
    public class Teams
    {
        private static Dictionary<StatType, float> statCoffs;

        private CharacterStats leader;
        private CharacterStats[] team;

        private TeamColor teamColor;

        public Teams(CharacterStats leader)
        {
            statCoffs = new Dictionary<StatType, float>();
            statCoffs.Add(StatType.Sing, 1);
            statCoffs.Add(StatType.Dance, 1);
            statCoffs.Add(StatType.Appearance, 1);
            statCoffs.Add(StatType.Charm, 1);
            this.leader = leader;
            team = new CharacterStats[Constant.MAX_TEAM_MEMBER];
            team[0] = leader;
        }

        public TeamColor GetTeamColor()
        {
            CharacterStats totalStat = TotalStat();
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
            sumValue /= Constant.MAX_TEAM_MEMBER - 1;

            if (IsOneManTeam())
            {
                return TeamColor.OneMan;
            }
            else if (maxStat.Value >= sumValue * Constant.TEAM_COLOR_COFF)
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

            foreach (CharacterStats stat in team)
            {
                totalStat.AddStat(stat);
            }

            return totalStat;
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
            int leaderSumStat = leader.SumStat();
            int teamSumStat = 0;
            foreach (CharacterStats stat in team)
            {
                teamSumStat += stat.SumStat();
            }

            teamSumStat -= leaderSumStat;

            return leaderSumStat > teamSumStat * Constant.TEAM_COLOR_COFF;
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
            return (float)(Constant.MAX_HARMONY_COFF -
                           (Constant.MAX_HARMONY_COFF - Constant.MIN_HARMONY_COFF) * normalize);
        }
    }
}