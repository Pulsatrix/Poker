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

        public CardMask Parse(string setOfCardValues)
        {
            CardMask cardMask;
            Parse(setOfCardValues, true, null, out cardMask);
            return cardMask;
        }

        public bool TryParse(string setOfCardValues, out CardMask cardMask)
        {
            var result = Parse(setOfCardValues, true, null, out cardMask);
            return result;
        }

        private bool Parse(string setOfCardValues, bool raiseException, ICollection<Card> cards, out CardMask cardMask)
        {
            cards?.Clear();

            cardMask = CardMask.Empty;

            if (setOfCardValues == null)
            {
                if (raiseException)
                {
                    throw new ArgumentNullException(nameof(setOfCardValues));
                }

                return false;
            }

            //var compareInfo = CultureInfo.CurrentCulture.CompareInfo;
            const int stateRank = 0x0001;
            const int stateSuit = 0x0002;
            var state = stateRank;

            foreach (var symbol in setOfCardValues.Where(c => !char.IsWhiteSpace(c)))
            {
                var cardRank = CardRank.Undefined;
                CardSuit cardSuit;

                switch (state)
                {
                    case stateRank:
                        //var rankIndex = compareInfo.IndexOf(AbbreviatedRankNames, symbol, CompareOptions.IgnoreCase);
                        var rankIndex = AbbreviatedRankNames.IndexOf(symbol.ToString());
                        cardRank = ToRank(rankIndex);
                        if (cardRank == CardRank.Undefined)
                        {
                            if (raiseException)
                            {
                                throw new ArgumentException(symbol.ToString(), nameof(setOfCardValues));
                            }

                            return false;
                        }

                        state = stateSuit;
                        continue;
                    case stateSuit:
                        //var suitIndex = compareInfo.IndexOf(AbbreviatedSuitNames, symbol, CompareOptions.IgnoreCase);
                        var suitIndex = AbbreviatedSuitNames.IndexOf(symbol.ToString());
                        cardSuit = ToSuit(suitIndex);
                        if (cardSuit == CardSuit.Undefined)
                        {
                            if (raiseException)
                            {
                                throw new ArgumentException(symbol.ToString(), nameof(setOfCardValues));
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
