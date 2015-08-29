using Poker.Deck;

namespace Poker.Evaluation
{
    public partial class StandardEvaluator : IEvaluator
    {
        private static readonly IRules StandardRules = new StandardRules();

        public int Evaluate(CardMask cardMask, int noOfCardsToEvaluate)
        {
            var handValue = HandValue.NothingHigh;

            var ss = cardMask.Spades();
            var sc = cardMask.Clubs();
            var sd = cardMask.Diamonds();
            var sh = cardMask.Hearts();
            var ranks = sc | sd | sh | ss;

            var rankCount = BitsTable[ranks];
            var duplicateCount = noOfCardsToEvaluate - rankCount;

            // Check for straight, flush, or straight flush, and return if we can determine
            // immediately that this is the best possible hand.
            if (rankCount >= 5)
            {
                if (BitsTable[ss] >= 5)
                {
                    if (StraightTable[ss] != 0)
                    {
                        handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.StraightFlush)) +
                            HandValue.FromTopCardRank(StraightTable[ss]);
                        return handValue;
                    }

                    handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.Flush)) +
                        TopFiveCardsTable[ss];
                }
                else if (BitsTable[sc] >= 5)
                {
                    if (StraightTable[sc] != 0)
                    {
                        handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.StraightFlush)) +
                            HandValue.FromTopCardRank(StraightTable[sc]);
                        return handValue;
                    }

                    handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.Flush)) +
                        TopFiveCardsTable[sc];
                }
                else if (BitsTable[sd] >= 5)
                {
                    if (StraightTable[sd] != 0)
                    {
                        handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.StraightFlush)) +
                            HandValue.FromTopCardRank(StraightTable[sd]);
                        return handValue;
                    }

                    handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.Flush)) +
                        TopFiveCardsTable[sd];
                }
                else if (BitsTable[sh] >= 5)
                {
                    if (StraightTable[sh] != 0)
                    {
                        handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.StraightFlush)) +
                            HandValue.FromTopCardRank(StraightTable[sh]);
                        return handValue;
                    }

                    handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.Flush)) +
                        TopFiveCardsTable[sh];
                }
                else
                {
                    var straight = StraightTable[ranks];
                    if (straight != 0)
                    {
                        handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.Straight)) +
                            HandValue.FromTopCardRank(straight);
                    }
                }

                // Another win - if there can't be a FH/Quads (duplicateCount < 3), which is true
                // most of the time when there is a made hand, then if we've found a five card hand,
                // just return. This skips the whole process of computing two_mask/three_mask/etc.
                if (handValue != HandValue.NothingHigh && duplicateCount < 3)
                {
                    return handValue;
                }
            }

            int twoMask, threeMask;

            // By the time we're here, either:
            // 1) there's no five-card hand possible (flush or straight), or
            // 2) there's a flush or straight, but we know that there are enough duplicates
            // to make a full house or quads possible.
            switch (duplicateCount)
            {
                // It's a no-pair hand.
                case 0:
                    handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.NoPair)) +
                        TopFiveCardsTable[ranks];
                    return handValue;

                // It's a one-pair hand.
                case 1:
                    twoMask = ranks ^ (sc ^ sd ^ sh ^ ss);
                    handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.OnePair)) +
                        HandValue.FromTopCardRank(TopCardTable[twoMask]);

                    // Only one bit set in two_mask.
                    twoMask ^= ranks;

                    // Get the top five cards in what is left, drop all but the top three cards,
                    // and shift them by one to get the three desired kickers.
                    var kickers = (TopFiveCardsTable[twoMask] >> HandValue.CardBitsWidth) & ~HandValue.FifthCardRankMask;
                    handValue += kickers;
                    return handValue;

                // Either two pair or trips.
                case 2:
                    twoMask = ranks ^ (sc ^ sd ^ sh ^ ss);
                    if (twoMask != HandValue.NothingHigh)
                    {
                        handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.TwoPair)) +
                            (TopFiveCardsTable[twoMask] & (HandValue.TopCardRankMask | HandValue.SecondCardRankMask));

                        // Exactly two bits set in two_mask.
                        twoMask ^= ranks;
                        handValue += HandValue.FromThirdCardRank(TopCardTable[twoMask]);
                        return handValue;
                    }

                    threeMask = ((sc & sd) | (sh & ss)) & ((sc & sh) | (sd & ss));
                    handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.Trips)) +
                        HandValue.FromTopCardRank(TopCardTable[threeMask]);

                    // Only one bit set in three_mask.
                    threeMask ^= ranks;
                    var secondRank = TopCardTable[threeMask];
                    handValue += HandValue.FromSecondCardRank(secondRank);

                    threeMask ^= 1 << secondRank;
                    handValue += HandValue.FromThirdCardRank(TopCardTable[threeMask]);
                    return handValue;

                // Possible quads, fullhouse, straight or flush, or two pair.
                default:
                    var fourMask = sh & sd & sc & ss;
                    int topCardRank;
                    if (fourMask != HandValue.NothingHigh)
                    {
                        topCardRank = TopCardTable[fourMask];
                        handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.Quads)) +
                            HandValue.FromTopCardRank(topCardRank);

                        fourMask = ranks ^ (1 << topCardRank);
                        handValue += HandValue.FromSecondCardRank(TopCardTable[fourMask]);
                        return handValue;
                    }

                    // Technically, three_mask as defined below is really the set of bits which
                    // are set in three or four of the suits, but since we've already eliminated
                    // quads, this is OK. Similarly, two_mask is really two_or_four_mask, but
                    // since we've already eliminated quads, we can use this shortcut.
                    twoMask = ranks ^ (sc ^ sd ^ sh ^ ss);
                    if (BitsTable[twoMask] != duplicateCount)
                    {
                        // Must be some trips then, which really means there is a full house
                        // since duplicateCount >= 3.
                        threeMask = ((sc & sd) | (sh & ss)) & ((sc & sh) | (sd & ss));
                        topCardRank = TopCardTable[threeMask];
                        handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.FullHouse)) +
                            HandValue.FromTopCardRank(topCardRank);
                        threeMask = (twoMask | threeMask) ^ (1 << topCardRank);
                        handValue += HandValue.FromSecondCardRank(TopCardTable[threeMask]);
                        return handValue;
                    }

                    // flush and straight.
                    if (handValue != HandValue.NothingHigh)
                    {
                        return handValue;
                    }

                    // Must be two pair
                    topCardRank = TopCardTable[twoMask];
                    handValue = HandValue.FromHandTypeRank(StandardRules.ToHandTypeRank(HandType.TwoPair)) +
                        HandValue.FromTopCardRank(topCardRank);

                    twoMask ^= 1 << topCardRank;
                    secondRank = TopCardTable[twoMask];
                    handValue += HandValue.FromSecondCardRank(secondRank);

                    twoMask = ranks ^ (1 << topCardRank) ^ (1 << secondRank);
                    handValue += HandValue.FromThirdCardRank(TopCardTable[twoMask]);
                    return handValue;
            }
        }
    }
}
