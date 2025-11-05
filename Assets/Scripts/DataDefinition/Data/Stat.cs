using Script.DataDefinition.Enum;

namespace Script.DataDefinition.Data
{
    public class Stat
    {
        public StatType StatType { get; private set; }
        public int Value { get; private set; }

        public Stat(StatType statType, int value)
        {
            StatType = statType;
            Value = value;
        }

        public void AddValue(int value)
        {
            Value += value;
        }
    }
}