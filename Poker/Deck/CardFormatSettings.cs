using System;

namespace Poker.Deck
{
    [Flags]
    public enum CardFormatSettings
    {
        None = 0,
        NotInitialized = 1,
        UseGenitiveSuit = 2,
        UseSuitFirst = 4
    }
}
