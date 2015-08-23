using System.Diagnostics;

namespace Poker.Deck
{
    public abstract class DeckBase : IDeck
    {
        [DebuggerStepThrough]
        protected DeckBase(int noOfCards, int noOfSuits, int noOfRanks)
        {
            NoOfCards = noOfCards;
            NoOfSuits = noOfSuits;
            NoOfRanks = noOfRanks;
        }

        public int NoOfCards { get; }

        public int NoOfSuits { get; }

        public int NoOfRanks { get; }

        public abstract CardRank FirstRank { get; }

        public int ToSuitIndex(int cardIndex)
        {
            var suitIndex = cardIndex/NoOfRanks;
            return suitIndex;
        }

        public abstract int ToSuitIndex(CardSuit cardSuit);

        public abstract CardSuit ToSuit(int suitIndex);

        public int ToRankIndex(int cardIndex)
        {
            var rankIndex = ToRankIndex(FirstRank) + cardIndex%NoOfRanks;
            return rankIndex;
        }

        public abstract int ToRankIndex(CardRank cardRank);

        public abstract CardRank ToRank(int rankIndex);

        public abstract CardMask ToCardMask(int cardIndex);
    }
}
