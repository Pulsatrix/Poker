using System;

namespace Poker.Deck
{
    public struct CardMask : IEquatable<CardMask>, IFormattable
    {
        public static readonly CardMask Empty = new CardMask(0L);

        private readonly long _cardMask;

        private CardMask(long cardMask)
        {
            _cardMask = cardMask;
        }

        public static explicit operator CardMask(long value) => new CardMask(value);

        public static CardMask FromInt64(long value) => new CardMask(value);

        public static explicit operator long(CardMask value) => value._cardMask;

        public static long ToInt64(CardMask value) => value._cardMask;

        public static bool operator ==(CardMask left, CardMask right) => left._cardMask == right._cardMask;

        public static bool operator !=(CardMask left, CardMask right) => !(left == right);

        public static bool operator ==(long left, CardMask right) => left == right._cardMask;

        public static bool operator !=(long left, CardMask right) => !(left == right);

        public static CardMask operator &(CardMask left, CardMask right)
            => new CardMask(left._cardMask & right._cardMask);

        public static CardMask BitwiseAnd(CardMask left, CardMask right) => left & right;

        public static CardMask operator |(CardMask left, CardMask right)
            => new CardMask(left._cardMask | right._cardMask);

        public static CardMask BitwiseOr(CardMask left, CardMask right) => left | right;

        public static CardMask operator <<(CardMask left, int right) => new CardMask(left._cardMask << right);

        public static CardMask LeftShift(CardMask left, int right) => left << right;

        public static CardMask operator >>(CardMask left, int right) => new CardMask(left._cardMask >> right);

        public static CardMask RightShift(CardMask left, int right) => left >> right;

        public override bool Equals(object obj) => obj is CardMask && Equals((CardMask) obj);

        public bool Equals(CardMask other) => this == other;

        public override int GetHashCode() => _cardMask.GetHashCode();

        internal int Spades() => (int) (_cardMask & 0xFFFFL);

        internal int Clubs() => (int) ((_cardMask & 0xFFFF0000L) >> 16);

        internal int Diamonds() => (int) ((_cardMask & 0xFFFF00000000L) >> 32);

        internal int Hearts() => (int) ((_cardMask & 0x7FFF000000000000L) >> 48);

        /// <summary>
        ///     Checks whether two Card Masks have at least one same card set in both Card Masks.
        /// </summary>
        /// <param name="value1">
        ///     The first card mask to check.
        /// </param>
        /// <param name="value2">
        ///     The second card mask to check.
        /// </param>
        /// <returns>
        ///     <c>true</c> if at least one same card set in both Card Masks.; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsAnySameCardSet(CardMask value1, CardMask value2) => (value1 & value2) != Empty;

        public int NoOfCardsSet()
        {
            var count = 0;
            var tempMask = new CardMask(1L);

            for (var i = 0; i != 64; ++i, tempMask <<= 1)
            {
                if ((this & tempMask) != Empty)
                {
                    ++count;
                }
            }

            return count;
        }

        public override string ToString() => ToString(null, CardFormatInfo.CurrentInfo);

        public string ToString(string format) => ToString(format, CardFormatInfo.CurrentInfo);

        public string ToString(IFormatProvider provider) => ToString(null, provider);

        public string ToString(string format, IFormatProvider formatProvider)
            => CardFormat.Format(this, format, CardFormatInfo.GetInstance(formatProvider));
    }
}
