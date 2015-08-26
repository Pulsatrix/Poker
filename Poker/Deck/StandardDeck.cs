namespace Poker.Deck
{
    public partial class StandardDeck : DeckBase
    {
        private const int CardCount = 52;
        private const int SuitCount = 4;
        private const int RankCount = 13;
        internal const int NoOfRankMasks = 1 << RankCount;
        private const string RankSymbolSet = "23456789TJQKA";
        private const string SuitSymbolSet = "HDCS";

        public StandardDeck() : base(CardCount, RankCount, SuitCount, RankSymbolSet, SuitSymbolSet)
        {
        }

        public override CardRank FirstRank => CardRank.Two;

        public override int ToRankIndex(CardRank cardRank)
        {
            int index;

            switch (cardRank)
            {
                case CardRank.Two:
                    index = 0;
                    break;
                case CardRank.Three:
                    index = 1;
                    break;
                case CardRank.Four:
                    index = 2;
                    break;
                case CardRank.Five:
                    index = 3;
                    break;
                case CardRank.Six:
                    index = 4;
                    break;
                case CardRank.Seven:
                    index = 5;
                    break;
                case CardRank.Eight:
                    index = 6;
                    break;
                case CardRank.Nine:
                    index = 7;
                    break;
                case CardRank.Ten:
                    index = 8;
                    break;
                case CardRank.Jack:
                    index = 9;
                    break;
                case CardRank.Queen:
                    index = 10;
                    break;
                case CardRank.King:
                    index = 11;
                    break;
                case CardRank.Ace:
                    index = 12;
                    break;
                case CardRank.Undefined:
                    index = -1;
                    break;
                default:
                    index = -1;
                    break;
            }

            return index;
        }

        public override CardRank ToRank(int rankIndex)
        {
            CardRank cardRank;

            switch (rankIndex)
            {
                case 0:
                    cardRank = CardRank.Two;
                    break;
                case 1:
                    cardRank = CardRank.Three;
                    break;
                case 2:
                    cardRank = CardRank.Four;
                    break;
                case 3:
                    cardRank = CardRank.Five;
                    break;
                case 4:
                    cardRank = CardRank.Six;
                    break;
                case 5:
                    cardRank = CardRank.Seven;
                    break;
                case 6:
                    cardRank = CardRank.Eight;
                    break;
                case 7:
                    cardRank = CardRank.Nine;
                    break;
                case 8:
                    cardRank = CardRank.Ten;
                    break;
                case 9:
                    cardRank = CardRank.Jack;
                    break;
                case 10:
                    cardRank = CardRank.Queen;
                    break;
                case 11:
                    cardRank = CardRank.King;
                    break;
                case 12:
                    cardRank = CardRank.Ace;
                    break;
                default:
                    cardRank = CardRank.Undefined;
                    break;
            }

            return cardRank;
        }

        public override int ToSuitIndex(CardSuit cardSuit)
        {
            int index;

            switch (cardSuit)
            {
                case CardSuit.Hearts:
                    index = 0;
                    break;
                case CardSuit.Diamonds:
                    index = 1;
                    break;
                case CardSuit.Clubs:
                    index = 2;
                    break;
                case CardSuit.Spades:
                    index = 3;
                    break;
                case CardSuit.Undefined:
                    index = -1;
                    break;
                default:
                    index = -1;
                    break;
            }

            return index;
        }

        public override CardSuit ToSuit(int suitIndex)
        {
            CardSuit cardSuit;

            switch (suitIndex)
            {
                case 0:
                    cardSuit = CardSuit.Hearts;
                    break;
                case 1:
                    cardSuit = CardSuit.Diamonds;
                    break;
                case 2:
                    cardSuit = CardSuit.Clubs;
                    break;
                case 3:
                    cardSuit = CardSuit.Spades;
                    break;
                default:
                    cardSuit = CardSuit.Undefined;
                    break;
            }

            return cardSuit;
        }

        public override int ToCardIndex(CardRank cardRank, CardSuit cardSuit)
        {
            int cardIndex;

            checked
            {
                cardIndex = (ToSuitIndex(cardSuit)*RankCount) + ToRankIndex(cardRank);
            }

            return cardIndex;
        }

        public override CardMask ToCardMask(int cardIndex) => CardMaskTable[cardIndex];
    }
}
