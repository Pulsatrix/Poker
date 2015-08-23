using System;
using System.Diagnostics;
using System.Globalization;

namespace Poker.Deck
{
    public struct CardMask : IEquatable<CardMask>
    {
        public static readonly CardMask Empty = new CardMask(0L);

        private readonly long _cardMask;

        [DebuggerStepThrough]
        public CardMask(long cardMask)
        {
            _cardMask = cardMask;
        }

        public static explicit operator CardMask(long value) => new CardMask(value);

        [DebuggerStepThrough]
        public static CardMask FromInt64(long value) => new CardMask(value);

        public static explicit operator long(CardMask value) => value._cardMask;

        [DebuggerStepThrough]
        public static long ToInt64(CardMask value) => value._cardMask;

        public static bool operator ==(CardMask left, CardMask right) => left._cardMask == right._cardMask;

        public static bool operator !=(CardMask left, CardMask right) => !(left == right);

        public static bool operator ==(long left, CardMask right) => left == right._cardMask;

        public static bool operator !=(long left, CardMask right) => !(left == right);

        public static CardMask operator &(CardMask left, CardMask right)
            => new CardMask(left._cardMask & right._cardMask);

        [DebuggerStepThrough]
        public static CardMask BitwiseAnd(CardMask left, CardMask right) => left & right;

        public static CardMask operator |(CardMask left, CardMask right)
            => new CardMask(left._cardMask | right._cardMask);

        [DebuggerStepThrough]
        public static CardMask BitwiseOr(CardMask left, CardMask right) => left | right;

        public static CardMask operator <<(CardMask left, int right) => new CardMask(left._cardMask << right);

        [DebuggerStepThrough]
        public static CardMask LeftShift(CardMask left, int right) => left << right;

        public static CardMask operator >>(CardMask left, int right) => new CardMask(left._cardMask >> right);

        public static CardMask RightShift(CardMask left, int right) => left >> right;

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
        [DebuggerStepThrough]
        public static bool IsAnySameCardSet(CardMask value1, CardMask value2) => (value1 & value2) != Empty;

        [DebuggerStepThrough]
        public override bool Equals(object obj) => obj is CardMask && Equals((CardMask) obj);

        [DebuggerStepThrough]
        public bool Equals(CardMask other) => this == other;

        [DebuggerStepThrough]
        public override int GetHashCode() => _cardMask.GetHashCode();

        [DebuggerStepThrough]
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

        [DebuggerStepThrough]
        public int Spades() => (int) (_cardMask & 0xFFFFL);

        [DebuggerStepThrough]
        public int Clubs() => (int) ((_cardMask & 0xFFFF0000L) >> 16);

        [DebuggerStepThrough]
        public int Diamonds() => (int) ((_cardMask & 0xFFFF00000000L) >> 32);

        [DebuggerStepThrough]
        public int Hearts() => (int) ((_cardMask & 0x7FFF000000000000L) >> 48);

        [DebuggerStepThrough]
        public override string ToString() => _cardMask.ToString(CultureInfo.InvariantCulture);
    }
}
