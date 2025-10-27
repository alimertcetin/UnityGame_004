using TMPro;
using XIV.Core.Utils;

namespace TheGame
{
    public static class TMPExtensions
    {
        public static void WriteScoreText(this TMP_Text txt, int score)
        {
            using var dispose = ArrayUtils.GetBuffer(out char[] buffer);
            if (ScoreTextFormatter.TryFormat(score, buffer, out int written))
            {
                txt.SetCharArray(buffer, 0, written);
            }
        }
    }
}