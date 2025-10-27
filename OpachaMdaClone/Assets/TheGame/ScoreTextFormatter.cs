using System;
using System.Runtime.CompilerServices;

namespace TheGame
{
    public static class ScoreTextFormatter
    {
        // Formats number like 950 -> "950", 1200 -> "1.2k", 2_000_000 -> "2M"
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryFormat(int value, Span<char> dest, out int written)
        {
            written = 0;

            if (value < 0)
                return false; // handle negative separately if needed

            // < 1k: just write integer
            if (value < 1_000)
                return value.TryFormat(dest, out written);

            // 1k..999_999
            if (value < 1_000_000)
            {
                int thousands = value / 1_000;
                int remainder = (value % 1_000) / 100; // tenths of a thousand (1 decimal)

                // Example: 1530 => thousands=1, remainder=5 => "1.5k"
                if (remainder == 0)
                {
                    if (!thousands.TryFormat(dest, out written)) return false;
                    if (written >= dest.Length) return false;
                    dest[written++] = 'k';
                }
                else
                {
                    if (!thousands.TryFormat(dest, out written)) return false;
                    if (written + 2 >= dest.Length) return false;
                    dest[written++] = '.';
                    dest[written++] = (char)('0' + remainder);
                    dest[written++] = 'k';
                }

                return true;
            }

            // 1M..999_999_999
            if (value < 1_000_000_000)
            {
                int millions = value / 1_000_000;
                int remainder = (value % 1_000_000) / 100_000; // tenths of a million

                if (remainder == 0)
                {
                    if (!millions.TryFormat(dest, out written)) return false;
                    if (written >= dest.Length) return false;
                    dest[written++] = 'M';
                }
                else
                {
                    if (!millions.TryFormat(dest, out written)) return false;
                    if (written + 2 >= dest.Length) return false;
                    dest[written++] = '.';
                    dest[written++] = (char)('0' + remainder);
                    dest[written++] = 'M';
                }

                return true;
            }

            // Billions
            {
                int billions = value / 1_000_000_000;
                int remainder = (value % 1_000_000_000) / 100_000_000;

                if (remainder == 0)
                {
                    if (!billions.TryFormat(dest, out written)) return false;
                    if (written >= dest.Length) return false;
                    dest[written++] = 'B';
                }
                else
                {
                    if (!billions.TryFormat(dest, out written)) return false;
                    if (written + 2 >= dest.Length) return false;
                    dest[written++] = '.';
                    dest[written++] = (char)('0' + remainder);
                    dest[written++] = 'B';
                }

                return true;
            }
        }
    }
}