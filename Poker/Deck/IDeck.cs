namespace Poker.Deck
{
    public interface IDeck
    {
        int NoOfCards { get; }

        int NoOfRanks { get; }

        int NoOfSuits { get; }

        CardRank FirstRank { get; }

        string SuitSymbols { get; set; }

        string RankSymbols { get; set; }

        int ToRankIndex(int cardIndex);

        int ToRankIndex(CardRank cardRank);

        CardRank ToRank(int rankIndex);

        int ToSuitIndex(int cardIndex);

        int ToSuitIndex(CardSuit cardSuit);

        CardSuit ToSuit(int suitIndex);

        int ToCardIndex(CardRank cardRank, CardSuit cardSuit);

        CardMask ToCardMask(int cardIndex);

        CardMask Parse(string setOfCardValues);

        bool TryParse(string setOfCardValues, out CardMask cardMask);
    }
}
