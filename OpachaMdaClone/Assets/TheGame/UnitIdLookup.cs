using XIV.Core.DataStructures;
using XIV.Core.Extensions;

namespace TheGame
{
    public static class UnitIdLookup
    {
        public const int MAX_UNIT_ID_LENGTH = 8;
        public static readonly XIVColor[] colors = new XIVColor[MAX_UNIT_ID_LENGTH]
        {
            XIVColor.green,
            XIVColor.blue,
            XIVColor.red,
            XIVColor.magenta,
            XIVColor.yellow,
            XIVColor.cyan,
            XIVColor.black,
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