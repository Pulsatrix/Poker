using System.Collections.Generic;

namespace Poker.Deck
{
    public interface IDeck
    {
        int NoOfCards { get; }

        int NoOfRanks { get; }

        int NoOfSuits { get; }

        CardRank FirstRank { get; }

        IList<string> RankNames { get; }

        IList<string> AbbreviatedRankNames { get; }

        IList<string> SuitNames { get; }

        IList<string> AbbreviatedSuitNames { get; }

        IList<string> GenitiveSuitNames { get; }

        int ToRankIndex(int cardIndex);

        int ToRankIndex(CardRank cardRank);

        CardRank ToRank(int rankIndex);

        int ToSuitIndex(int cardIndex);

        int ToSuitIndex(CardSuit cardSuit);

        CardSuit ToSuit(int suitIndex);

        int ToCardIndex(CardRank cardRank, CardSuit cardSuit);

        IEnumerable<int> ToCardIndexes(CardMask cardMask);

        CardMask ToCardMask(int cardIndex);

        CardMask ParseCards(string value);

        bool TryParseCards(string value, out CardMask cardMask);

        CardRank ParseCardRank(char value);

        CardSuit ParseCardSuit(char value);
    }
}
