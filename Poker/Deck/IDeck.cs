namespace Poker.Deck
{
    public interface IDeck
    {
        int NoOfCards { get; }

        int NoOfSuits { get; }

        int NoOfRanks { get; }

        CardRank FirstRank { get; }

        int ToSuitIndex(int cardIndex);

        int ToSuitIndex(CardSuit cardSuit);

        CardSuit ToSuit(int suitIndex);

        int ToRankIndex(int cardIndex);

        int ToRankIndex(CardRank cardRank);

        CardRank ToRank(int rankIndex);

        CardMask ToCardMask(int cardIndex);
    }
}
