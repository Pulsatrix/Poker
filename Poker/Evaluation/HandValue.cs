using System;
using System.Globalization;
using Poker.Deck;

namespace Poker.Evaluation
{
    public struct HandValue : IEquatable<HandValue>, IFormattable
    {
        private static readonly IRules StandardRules = new StandardRules();
        private static readonly IDeck StandardDeck = new StandardDeck();

        public static readonly int NothingLow = FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.StraightFlush)) +
            FromTopCardRank(StandardDeck.ToRankIndex(CardRank.Ace)) + 1;

        public const int NothingHigh = 0;
        public const int CardBitsWidth = 4;

        public const int HandTypeRankCardMask = 0x0F000000;
        public const int TopCardRankMask = 0x000F0000;
        public const int SecondCardRankMask = 0x0000F000;
        public const int ThirdCardRankMask = 0x00000F00;
        public const int FourthCardRankMask = 0x000000F0;
        public const int FifthCardRankMask = 0x0000000F;

        private const int HandTypeRankShift = 24;
        private const int TopCardRankShift = 16;
        private const int SecondCardRankShift = 12;
        private const int ThirdCardRankShift = 8;
        private const int FourthCardRankShift = 4;
        private const int FifthCardRankShift = 0;

        public static readonly HandValue Nothing = new HandValue(NothingHigh, NothingLow);

        public HandValue(int highValue, int lowValue)
        {
            HighValue = highValue;
            LowValue = lowValue;
        }

        public static bool operator ==(HandValue left, HandValue right)
            => left.HighValue == right.HighValue && left.LowValue == right.LowValue;

        public static bool operator !=(HandValue left, HandValue right) => !(left == right);

        public int HighValue { get; set; }

        public int LowValue { get; set; }

        public override bool Equals(object obj) => obj is HandValue && Equals((HandValue) obj);

        public bool Equals(HandValue other) => this == other;

        public override int GetHashCode()
        {
            unchecked
            {
                return (HighValue.GetHashCode()*397) ^ LowValue.GetHashCode();
            }
        }

        public override string ToString() => ToString(null, CultureInfo.InvariantCulture);

        public string ToString(string format, IFormatProvider formatProvider)
            =>
                string.Format(CultureInfo.CurrentCulture,
                    "{0},{1}",
                    HighValue.ToString(format, formatProvider),
                    LowValue.ToString(format, formatProvider));

        public static int FromHandTypeRank(int handTypeRank) => handTypeRank << HandTypeRankShift;

        public static int FromTopCardRank(int cardRank) => cardRank << TopCardRankShift;

        public static int FromSecondCardRank(int cardRank) => cardRank << SecondCardRankShift;

        public static int FromThirdCardRank(int cardRank) => cardRank << ThirdCardRankShift;

        public static int FromFourthCardRank(int cardRank) => cardRank << FourthCardRankShift;

        public static int FromFifthCardRank(int cardRank) => cardRank << FifthCardRankShift;
    }
}
