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

        public CharacterStats(CharacterStats stat)
        {
            Sing = new Stat(StatType.Sing, stat.Sing.Value);
            Dance = new Stat(StatType.Dance, stat.Dance.Value);
            Appearance = new Stat(StatType.Appearance, stat.Appearance.Value);
            Charm = new Stat(StatType.Charm, stat.Charm.Value);
        }

        public void AddStat(CharacterStats stat)
        {
            Sing.AddValue(stat.Sing.Value);
            Dance.AddValue(stat.Dance.Value);
            Appearance.AddValue(stat.Appearance.Value);
            Charm.AddValue(stat.Charm.Value);
        }

        public void NewStat(StatType statType, int value)
        {
            Stat stat = new Stat(statType, value);
            switch (statType)
            {
                case StatType.Sing:
                    Sing = stat;
                    break;
                case StatType.Dance:
                    Dance = stat;
                    break;
                case StatType.Appearance:
                    Appearance = stat;
                    break;
                case StatType.Charm:
                    Charm = stat;
                    break;
                default:
                    break;
            }
        }

        public void SetRandomStat()
        {
            DiceRoller.RollDice(DiceRoller.SIX_DICE_EYE);
            Sing = new Stat(StatType.Sing, DiceRoller.lastRollResult);
            DiceRoller.RollDice(DiceRoller.SIX_DICE_EYE);
            Dance = new Stat(StatType.Dance, DiceRoller.lastRollResult);
            DiceRoller.RollDice(DiceRoller.SIX_DICE_EYE);
            Appearance = new Stat(StatType.Appearance, DiceRoller.lastRollResult);
            DiceRoller.RollDice(DiceRoller.SIX_DICE_EYE);
            Charm = new Stat(StatType.Charm, DiceRoller.lastRollResult);
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

        public override string ToString()
        {
            return
                $"Sing: {Sing.Value:D2} | Dance: {Dance.Value:D2} | Appearance: {Appearance.Value:D2} | Charm: {Charm.Value:D2}";
        }
    }
}