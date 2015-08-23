using Poker.Deck;

namespace Poker.Evaluation
{
    internal static class EvaluatorTablesHelper
    {
        internal static int[] GenerateStandardEvaluatorBitsTable()
        {
            var bitsTable = new int[StandardDeck.NoOfRankMasks];

            for (var rankMaskIndex = 0; rankMaskIndex != StandardDeck.NoOfRankMasks; ++rankMaskIndex)
            {
                var rankMask = 0;
                var n = rankMaskIndex;

                while (n != 0)
                {
                    if ((n & 1) != 0)
                    {
                        ++rankMask;
                    }

                    n >>= 1;
                }

                bitsTable[rankMaskIndex] = rankMask;
            }

            return bitsTable;
        }

        internal static int[] GenerateStandardEvaluatorStraightTable()
        {
            var straightTable = new int[StandardDeck.NoOfRankMasks];

            for (var rankMaskIndex = 0; rankMaskIndex != StandardDeck.NoOfRankMasks; ++rankMaskIndex)
            {
                var rankMask = GetStraightRankMask(rankMaskIndex);
                straightTable[rankMaskIndex] = rankMask;
            }

            return straightTable;
        }

        internal static int[] GenerateStandardEvaluatorTopCardTable()
        {
            var topCardTable = new int[StandardDeck.NoOfRankMasks];

            for (var rankMaskIndex = 0; rankMaskIndex != StandardDeck.NoOfRankMasks; ++rankMaskIndex)
            {
                var rankMask = GetTopCardRankMask(rankMaskIndex);
                topCardTable[rankMaskIndex] = rankMask;
            }

            return topCardTable;
        }

        internal static int[] GenerateStandardEvaluatorTopFiveCardsTable()
        {
            var topFiveCardTable = new int[StandardDeck.NoOfRankMasks];

            for (var rankMaskIndex = 0; rankMaskIndex != StandardDeck.NoOfRankMasks; ++rankMaskIndex)
            {
                var rankMask = 0;
                var n = rankMaskIndex;

                for (var j = 0; j != 5; ++j)
                {
                    rankMask <<= HandValue.CardBitsWidth;
                    var card = GetTopCardRankMask(n);
                    rankMask += card;
                    n &= ~(1 << card);
                }

                topFiveCardTable[rankMaskIndex] = rankMask;
            }

            return topFiveCardTable;
        }

        private static int GetStraightRankMask(int ranks)
        {
            var ranks1 = ranks & (ranks << 1);
            var ranks2 = ranks1 & (ranks << 2);
            var ranks3 = ranks2 & (ranks << 3);
            var ranks4 = ranks3 & (ranks << 4);

            if (ranks1 != 0 && ranks2 != 0 && ranks3 != 0 && ranks4 != 0)
            {
                var rankMask = GetTopCardRankMask(ranks4);
                return rankMask;
            }

            var deck = new StandardDeck();
            var aceRankIndex = deck.ToRankIndex(CardRank.Ace);
            var twoRankIndex = deck.ToRankIndex(CardRank.Two);
            var threeRankIndex = deck.ToRankIndex(CardRank.Three);
            var fourRankIndex = deck.ToRankIndex(CardRank.Four);
            var fiveRankIndex = deck.ToRankIndex(CardRank.Five);
            var standardRulesFiveStraight = (1 << aceRankIndex) | (1 << twoRankIndex) | (1 << threeRankIndex) |
                (1 << fourRankIndex) | (1 << fiveRankIndex);

            return (ranks & standardRulesFiveStraight) == standardRulesFiveStraight
                ? deck.ToRankIndex(CardRank.Five)
                : 0;
        }

        private static int GetTopCardRankMask(int ranks)
        {
            if (ranks == 0)
                return 0;

            var aceRankIndex = (new StandardDeck()).ToRankIndex(CardRank.Ace);

            var bit = 1 << aceRankIndex;
            var rankMask = aceRankIndex;

            while ((ranks & bit) == 0)
            {
                bit >>= 1;
                --rankMask;
            }

            return rankMask;
        }
    }
}
