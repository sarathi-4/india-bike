using System.Collections.Generic;

namespace IndianBikeLife.Communication
{
    /// <summary>
    /// Indian-themed quick chat phrase catalogue for rapid hands-on-handlebar communication.
    /// </summary>
    public static class QuickChatController
    {
        public static readonly string[] IndianRidingPhrases = new string[]
        {
            "Horn OK Please!",
            "Take the Ghat Road ahead!",
            "Fuel stop at the Dhaba!",
            "Slow down — Speed breaker ahead!",
            "Wait for pillion!",
            "Follow my line!",
            "Nice motorcycle!",
            "Let's race to the checkpoint!"
        };

        public static string GetPhrase(int index)
        {
            if (index >= 0 && index < IndianRidingPhrases.Length)
            {
                return IndianRidingPhrases[index];
            }
            return "Horn OK Please!";
        }

        public static int PhraseCount => IndianRidingPhrases.Length;
    }
}

