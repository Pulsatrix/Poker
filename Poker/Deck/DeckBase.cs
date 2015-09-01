using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Poker.Deck
{
    public abstract class DeckBase : IDeck
    {
        protected DeckBase(int noOfCards, int noOfRanks, int noOfSuits)
        {
            NoOfCards = noOfCards;
            NoOfRanks = noOfRanks;
            NoOfSuits = noOfSuits;
        }

        public int NoOfCards { get; }

        public int NoOfSuits { get; }

        public int NoOfRanks { get; }

        public abstract CardRank FirstRank { get; }

        public IList<string> RankNames { get; } = new List<string>();

        public IList<string> AbbreviatedRankNames { get; } = new List<string>();

        public IList<string> SuitNames { get; } = new List<string>();

        public IList<string> AbbreviatedSuitNames { get; } = new List<string>();

        public IList<string> GenitiveSuitNames { get; } = new List<string>();

        public int ToRankIndex(int cardIndex)
        {
            var rankIndex = ToRankIndex(FirstRank) + cardIndex%NoOfRanks;
            return rankIndex;
        }

        public int ToSuitIndex(int cardIndex)
        {
            var suitIndex = cardIndex/NoOfRanks;
            return suitIndex;
        }

        public abstract int ToRankIndex(CardRank cardRank);

        public abstract CardRank ToRank(int rankIndex);

        public abstract int ToSuitIndex(CardSuit cardSuit);

        public abstract CardSuit ToSuit(int suitIndex);

        public abstract int ToCardIndex(CardRank cardRank, CardSuit cardSuit);

        public abstract IEnumerable<int> ToCardIndexes(CardMask cardMask);

        public abstract CardMask ToCardMask(int cardIndex);

        public CardMask ParseCards(string value)
        {
            CardMask cardMask;
            Parse(value, true, null, out cardMask);
            return cardMask;
        }

        public bool TryParseCards(string value, out CardMask cardMask)
        {
            var result = Parse(value, true, null, out cardMask);
            return result;
        }

        public CardRank ParseCardRank(char value)
        {
            var rankIndex = AbbreviatedRankNames.IndexOf(value.ToString());
            var cardRank = ToRank(rankIndex);
            return cardRank;
        }

        public CardSuit ParseCardSuit(char value)
        {
            var suitIndex = AbbreviatedSuitNames.IndexOf(value.ToString());
            var cardSuit = ToSuit(suitIndex);
            return cardSuit;
        }

        private bool Parse(string value, bool raiseException, ICollection<Card> cards, out CardMask cardMask)
        {
            cards?.Clear();

            cardMask = CardMask.Empty;

            if (value == null)
            {
                if (raiseException)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                return false;
            }

            const int stateRank = 0x0001;
            const int stateSuit = 0x0002;
            var state = stateRank;

            foreach (var symbol in value.Where(c => !char.IsWhiteSpace(c)))
            {
                var cardRank = CardRank.Undefined;
                CardSuit cardSuit;

                switch (state)
                {
                    case stateRank:
                        cardRank = ParseCardRank(symbol);
                        if (cardRank == CardRank.Undefined)
                        {
                            if (raiseException)
                            {
                                throw new ArgumentException(symbol.ToString(), nameof(value));
                            }

                            return false;
                        }

                        state = stateSuit;
                        continue;
                    case stateSuit:
                        cardSuit = ParseCardSuit(symbol);
                        if (cardSuit == CardSuit.Undefined)
                        {
                            if (raiseException)
                            {
                                throw new ArgumentException(symbol.ToString(), nameof(value));
                            }

                            return false;
                        }

                        state = stateRank;
                        break;
                    default:
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                            "{0}={1}",
                            nameof(state),
                            state));
                }

                if (cards != null)
                {
                    var card = new Card(cardRank, cardSuit);
                    if (!cards.Contains(card))
                    {
                        cards.Add(card);
                    }
                }

                var cardIndex = ToCardIndex(cardRank, cardSuit);
                cardMask |= ToCardMask(cardIndex);
            }

            return true;
        }
    }
}
