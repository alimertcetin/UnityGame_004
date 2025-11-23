using TMPro;
using XIV.Core.Utils;

namespace TheGame
{
    public static class TMPExtensions
    {
        public static void WriteScoreText(this TMP_Text txt, int score)
        {
            using var buffer = ArrayUtils.GetBuffer<char>(16);
            if (ScoreTextFormatter.TryFormat(score, (char[])buffer, out int written))
            {
                txt.SetCharArray(buffer, 0, written);
            }
        }
    }
}