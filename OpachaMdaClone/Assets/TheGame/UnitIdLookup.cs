using XIV.Core.DataStructures;
using XIV.Core.Extensions;

namespace TheGame
{
    public static class UnitIdLookup
    {
        public const int MAX_UNIT_ID_LENGTH = 8;
        public static readonly XIVColor[] colors = new XIVColor[MAX_UNIT_ID_LENGTH]
        {
            new XIVColor(0.2f, 0.7f, 0.3f), // green
            new XIVColor(0.2f, 0.4f, 0.9f), // blue
            new XIVColor(0.9f, 0.3f, 0.3f), // red
            new XIVColor(0.8f, 0.3f, 0.8f), // magenta
            new XIVColor(0.95f, 0.85f, 0.3f), // yellow
            new XIVColor(0.95f, 0.85f, 0.3f),// cyan
            new XIVColor(0.05f, 0.05f, 0.05f), // black
            XIVColor.white,
        };

        public enum UnitType
        {
            Green = 0,
            Blue,
            Red,
            Magenta,
            Yellow,
            Cyan,
            Black,
            White,
            NumberOfItems
        }
        
        public static XIVColor GetColor(UnitType unitType)
        {
            return colors[(int)unitType];
        }
        
        public static UnitType GetUnitType(XIVColor color)
        {
            return (UnitType)colors.XIVIndexOf(p => p == color);
        }
    }
}