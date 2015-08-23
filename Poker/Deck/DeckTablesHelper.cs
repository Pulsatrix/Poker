namespace Poker.Deck
{
    internal static class DeckTablesHelper
    {
        internal static CardMask[] GenerateStandardDeckCardMaskTable()
        {
            var deck = new StandardDeck();
            var cardMaskTable = new CardMask[deck.NoOfCards];

            for (var cardIndex = 0; cardIndex != deck.NoOfCards; ++cardIndex)
            {
                var cardSuit = deck.ToSuit(deck.ToSuitIndex(cardIndex));
                var cardRankIndex = deck.ToRankIndex(cardIndex);
                CardMask cardMask;

                switch (cardSuit)
                {
                    case CardSuit.Hearts:
                        cardMask = (CardMask)((0x1L << cardRankIndex) << 48);
                        break;
                    case CardSuit.Diamonds:
                        cardMask = (CardMask)((0x1L << cardRankIndex) << 32);
                        break;
                    case CardSuit.Clubs:
                        cardMask = (CardMask)((0x1L << cardRankIndex) << 16);
                        break;
                    case CardSuit.Spades:
                        cardMask = (CardMask)(0x1L << cardRankIndex);
                        break;
                    case CardSuit.Undefined:
                        cardMask = CardMask.Empty;
                        break;
                    default:
                        cardMask = CardMask.Empty;
                        break;
                }

                cardMaskTable[cardIndex] = cardMask;
            }

            return cardMaskTable;
        }
    }
}
