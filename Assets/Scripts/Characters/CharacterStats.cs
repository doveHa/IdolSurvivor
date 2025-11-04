using Script.DataDefinition.Data;
using Script.DataDefinition.Enum;
using UnityEngine;

namespace Script.Characters
{
    public class CharacterStats
    {
        public Stat Sing { get; private set; }
        public Stat Dance { get; private set; }
        public Stat Appearance { get; private set; }
        public Stat Charm { get; private set; }

        public CharacterStats()
        {
            Sing = new Stat(StatType.Sing, 0);
            Dance = new Stat(StatType.Dance, 0);
            Appearance = new Stat(StatType.Appearance, 0);
            Charm = new Stat(StatType.Charm, 0);
        }

        public CharacterStats(Stat sing, Stat dance, Stat appearance, Stat charm)
        {
            Sing = sing;
            Dance = dance;
            Appearance = appearance;
            Charm = charm;
        }

        public void AddStat(CharacterStats stat)
        {
            Sing.AddValue(stat.Sing.Value);
            Dance.AddValue(stat.Dance.Value);
            Appearance.AddValue(stat.Appearance.Value);
            Charm.AddValue(stat.Charm.Value);
        }

        public void AddSing(int sing)
        {
            Sing.AddValue(sing);
        }

        public void AddDance(int dance)
        {
            Dance.AddValue(dance);
        }

        public void AddAppearance(int appearance)
        {
            Appearance.AddValue(appearance);
        }

        public void AddCharm(int charm)
        {
            Charm.AddValue(charm);
        }

        public int SumStat()
        {
            return Sing.Value + Dance.Value + Appearance.Value + Charm.Value;
        }

        public Stat[] ToStatArray()
        {
            return new Stat[] { Sing, Dance, Appearance, Charm };
        }

        public int[] ToValueArray()
        {
            return new int[] { Sing.Value, Dance.Value, Appearance.Value, Charm.Value, };
        }
    }
}