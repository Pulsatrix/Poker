using System.Collections.Generic;

namespace Poker.Deck
{
    public partial class StandardDeck : DeckBase
    {
        private const int CardCount = 52;
        private const int RankCount = 13;
        private const int SuitCount = 4;
        internal const int NoOfRankMasks = 1 << RankCount;

        private static readonly string[] RankNameArray = {
            "Two",
            "Three",
            "Four",
            "Five",
            "Six",
            "Seven",
            "Eight",
            "Nine",
            "Ten",
            "Jack",
            "Queen",
            "King",
            "Ace"
        };

        private static readonly string[] AbbreviatedRankNameArray = {
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "T",
            "J",
            "Q",
            "K",
            "A"
        };

        private static readonly string[] SuitNameArray = {
            "Hearts",
            "Diamonds",
            "Clubs",
            "Spades",
        };

        private static readonly string[] AbbreviatedSuitNameArray = {
            "h",
            "d",
            "c",
            "s",
        };

        private static readonly string[] GenitiveSuitNameArray = {
            "of Hearts",
            "of Diamonds",
            "of Clubs",
            "of Spades",
        };

        private static volatile IDeck _defaultInstance;

        public StandardDeck() : base(CardCount, RankCount, SuitCount)
        {
            RankNames.Clear();
            foreach (var s in RankNameArray)
            {
                RankNames.Add(s);
            }

            AbbreviatedRankNames.Clear();
            foreach (var s in AbbreviatedRankNameArray)
            {
                AbbreviatedRankNames.Add(s);
            }

            SuitNames.Clear();
            foreach (var s in SuitNameArray)
            {
                SuitNames.Add(s);
            }

            AbbreviatedSuitNames.Clear();
            foreach (var s in AbbreviatedSuitNameArray)
            {
                AbbreviatedSuitNames.Add(s);
            }

            GenitiveSuitNames.Clear();
            foreach (var s in GenitiveSuitNameArray)
            {
                GenitiveSuitNames.Add(s);
            }
        }

        public static IDeck DefaultInstance => _defaultInstance ?? (_defaultInstance = new StandardDeck());
        
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

        public override IEnumerable<int> ToCardIndexes(CardMask cardMask)
        {
            var cardIndexes = new int[cardMask.NoOfCardsSet()];
            var index = 0;

            var patternMask = (CardMask) 1L;

            for (var i = 0; i != 64; ++i, patternMask <<= 1)
            {
                var tempMask = cardMask & patternMask;
                if (tempMask == CardMask.Empty)
                {
                    continue;
                }

                for (var cardIndex = 0; cardIndex != CardMaskTable.Length; ++cardIndex)
                {
                    if (tempMask != CardMaskTable[cardIndex])
                    {
                        continue;
                    }

                    cardIndexes[index] = cardIndex;
                    ++index;
                    break;
                }
            }

            return cardIndexes;
        }

        public override CardMask ToCardMask(int cardIndex) => CardMaskTable[cardIndex];
    }
}
