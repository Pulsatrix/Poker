using System;
using System.Globalization;

namespace Poker.Deck
{
    public struct Card : IEquatable<Card>, IFormattable
    {
        public static readonly Card Undefined = new Card(CardRank.Undefined, CardSuit.Undefined);

        private readonly CardRank _cardRank;

        private readonly CardSuit _cardSuit;

        public Card(CardRank cardRank, CardSuit cardSuit)
        {
            _cardRank = cardRank;
            _cardSuit = cardSuit;
        }

        public static bool operator ==(Card left, Card right)
            => left._cardRank == right._cardRank && left._cardSuit == right._cardSuit;

        public static bool operator !=(Card left, Card right) => !(left == right);

        public override bool Equals(object obj) => obj is Card && Equals((Card) obj);

        public bool Equals(Card other) => this == other;

        public override int GetHashCode()
        {
            unchecked
            {
                return (_cardRank.GetHashCode()*397) ^ _cardSuit.GetHashCode();
            }
        }

        public override string ToString() => ToString(null, CultureInfo.InvariantCulture);

        public string ToString(string format, IFormatProvider formatProvider)
            => string.Format(CultureInfo.CurrentCulture, "{0},{1}", _cardRank.ToString(), _cardSuit.ToString());
    }
}
