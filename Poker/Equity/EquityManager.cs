using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Poker.Deck;
using Poker.Enumeration;
using Poker.Evaluation;

namespace Poker.Equity
{
    public static class EquityManager
    {
        private const char DefaultCardSetDelimiter = ',';
        private const char DefaultSlice = '-';
        private const char DefaultPlus = '+';
        private const char DefaultConnectorSuit = 's';
        private const char DefaultConnectorOffSuit = 'o';
        private const string DefaultRandomSet = "XxXx";

        public static void EnumerateAndEvaluate(ICalculator calculator,
            string boardSet,
            string deadSet,
            IEnumerable<string> pocketsSet)
        {
            if (calculator == null)
            {
                throw new ArgumentNullException(nameof(calculator));
            }

            var deck = calculator.Deck;
            var boardCardMask = deck.ParseCards(boardSet);
            var deadCardMask = deck.ParseCards(deadSet);

            var pocketsCardMasks = new Collection<CardMask>();
            foreach (var pocketsCardMask in pocketsSet.Select(pockets => deck.ParseCards(pockets)))
            {
                pocketsCardMasks.Add(pocketsCardMask);
            }

            EnumerateAndEvaluate(calculator, boardCardMask, deadCardMask, pocketsCardMasks);
        }

        public static void EnumerateAndEvaluate(ICalculator calculator,
            CardMask boardCardMask,
            CardMask deadCardMask,
            IEnumerable<CardMask> pocketsCardMasks)
        {
            if (calculator == null)
            {
                throw new ArgumentNullException(nameof(calculator));
            }

            var pockets = pocketsCardMasks.ToList();
            deadCardMask = CombineCardMasks(boardCardMask, deadCardMask, pockets);

            var deckEnumerator = calculator.GetEnumerator(boardCardMask, deadCardMask);
            var isAvailable = deckEnumerator.MoveNext();

            if (isAvailable)
            {
                do
                {
                    var enumeratedCardMask = deckEnumerator.Current;
                    Evaluate(calculator, boardCardMask, enumeratedCardMask, pockets);
                } while (deckEnumerator.MoveNext());
            }
            else
            {
                Evaluate(calculator, boardCardMask, CardMask.Empty, pockets);
            }
        }

        private static CardMask CombineCardMasks(CardMask boardCardMask,
            CardMask deadCardMask,
            IEnumerable<CardMask> pocketsCardMasks)
        {
            deadCardMask |= boardCardMask;
            deadCardMask = pocketsCardMasks.Aggregate(deadCardMask,
                (current, pocketsCardMask) => current | pocketsCardMask);
            return deadCardMask;
        }

        private static IEnumerable<EvaluationResult> Evaluate(ICalculator calculator,
            CardMask boardCardMask,
            CardMask enumeratedCardMask,
            IEnumerable<CardMask> pocketsCardMasks)
        {
            var evaluationResults = new Collection<EvaluationResult>();
            var bestHandValue = HandValue.Nothing;
            var highShare = 0;
            var lowShare = 0;

            foreach (var pocketsCardMask in pocketsCardMasks)
            {
                var handValue = calculator.Evaluate(boardCardMask, enumeratedCardMask, pocketsCardMask);
                var evaluationResult = new EvaluationResult {HandValue = handValue};

                if (handValue.HighValue != HandValue.NothingHigh)
                {
                    if (handValue.HighValue > bestHandValue.HighValue)
                    {
                        bestHandValue.HighValue = handValue.HighValue;
                        highShare = 1;
                    }
                    else if (handValue.HighValue == bestHandValue.HighValue)
                    {
                        ++highShare;
                    }
                }

                if (handValue.LowValue != HandValue.NothingLow)
                {
                    if (handValue.LowValue < bestHandValue.LowValue)
                    {
                        bestHandValue.LowValue = handValue.LowValue;
                        lowShare = 1;
                    }
                    else if (handValue.LowValue == bestHandValue.LowValue)
                    {
                        ++lowShare;
                    }
                }

                evaluationResults.Add(evaluationResult);
            }

            CalculateExpectedValue(bestHandValue, highShare, lowShare, evaluationResults);

            return evaluationResults;
        }

        private static void CalculateExpectedValue(HandValue bestHandValue,
            int highShare,
            int lowShare,
            IEnumerable<EvaluationResult> evaluationResults)
        {
            float highPot, lowPot;

            // Award pot fractions to winning hands.
            if (bestHandValue.HighValue != HandValue.NothingHigh && bestHandValue.LowValue != HandValue.NothingLow)
            {
                highPot = 0.5F/highShare;
                lowPot = 0.5F/lowShare;
            }
            else
            {
                highPot = bestHandValue.HighValue != HandValue.NothingHigh ? 1.0F/highShare : 0F;
                lowPot = bestHandValue.LowValue != HandValue.NothingLow ? 1.0F/lowShare : 0F;
            }

            foreach (var evaluationResult in evaluationResults)
            {
                var handValue = evaluationResult.HandValue;
                var potFraction = 0F;

                if (handValue.HighValue != HandValue.NothingHigh)
                {
                    if (handValue.HighValue == bestHandValue.HighValue)
                    {
                        potFraction += highPot;
                        if (highShare == 1)
                        {
                            ++evaluationResult.HighWinCount;
                        }
                        else
                        {
                            ++evaluationResult.HighTieCount;
                        }
                    }
                    else
                    {
                        ++evaluationResult.HighLoseCount;
                    }
                }

                if (handValue.LowValue != HandValue.NothingLow)
                {
                    if (handValue.LowValue == bestHandValue.LowValue)
                    {
                        potFraction += lowPot;
                        if (lowShare == 1)
                        {
                            ++evaluationResult.LowWinCount;
                        }
                        else
                        {
                            ++evaluationResult.LowTieCount;
                        }
                    }
                    else
                    {
                        ++evaluationResult.LowLoseCount;
                    }
                }

                if (potFraction > 0.99F)
                {
                    ++evaluationResult.ScoopCount;
                }

                evaluationResult.ExpectedValue += potFraction;
            }
        }

        public static PocketsDistribution ParsePocketsDistribution(string value, CardMask deadMask, IDeck deck)
        {
            if (deck == null)
            {
                throw new ArgumentNullException(nameof(deck));
            }

            var pocketsDistribution = new PocketsDistribution();

            if (string.IsNullOrEmpty(value))
            {
                return pocketsDistribution;
            }

            var split = value.Split(DefaultCardSetDelimiter);
            foreach (var token in split)
            {
                CardMask cardMask;
                if (string.IsNullOrEmpty(token) || token.Equals(DefaultRandomSet))
                {
                    AddRandomHand(deadMask, deck, pocketsDistribution);
                }
                else if (deck.TryParseCards(token, out cardMask))
                {
                    if (!pocketsDistribution.PocketsCardMasks.Contains(cardMask))
                    {
                        pocketsDistribution.PocketsCardMasks.Add(cardMask);
                    }
                }
                else
                {
                    ParseAgnosticHand(token, deadMask, deck, pocketsDistribution);
                }
            }

            return pocketsDistribution;
        }

        /// <summary>
        ///     Take the "XxXx" (random/unknown) agnostic hand and convert it to it's specific
        ///     constituent hands. If no dead cards are specified, a random hand always contains
        ///     1,326 possibilities. If one or more dead cards are specified, that number will be
        ///     less.
        /// </summary>
        private static void AddRandomHand(CardMask deadMask, IDeck deck, PocketsDistribution pocketsDistribution)
        {
            using (var cardExhaustiveCollection = new ExhaustiveDeckEnumerator(deck, 2, deadMask))
            {
                while (cardExhaustiveCollection.MoveNext())
                {
                    if (!pocketsDistribution.PocketsCardMasks.Contains(cardExhaustiveCollection.Current))
                    {
                        pocketsDistribution.PocketsCardMasks.Add(cardExhaustiveCollection.Current);
                    }
                }
            }
        }

        /// <summary>
        ///     Take a given agnostic hand, such as "AA" or "QJs+" or "TT-77", along
        ///     with an optional collection of "dead" cards, and boil it down into its
        ///     constituent specific Hold'em hands.
        /// </summary>
        private static void ParseAgnosticHand(string value,
            CardMask deadMask,
            IDeck deck,
            PocketsDistribution pocketsDistribution)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var slice = value.IndexOf(DefaultSlice);
            var plus = value.IndexOf(DefaultPlus);

            int floor1, floor2, ceil1, ceil2;

            if (slice > 0)
            {
                floor1 = deck.ToRankIndex(deck.ParseCardRank(value[slice + 1]));
                floor2 = deck.ToRankIndex(deck.ParseCardRank(value[slice + 2]));
                ceil1 = deck.ToRankIndex(deck.ParseCardRank(value[0]));
                ceil2 = deck.ToRankIndex(deck.ParseCardRank(value[1]));
            }
            else
            {
                floor1 = deck.ToRankIndex(deck.ParseCardRank(value[0]));
                floor2 = deck.ToRankIndex(deck.ParseCardRank(value[1]));
                ceil1 = plus > 0 ? deck.ToRankIndex(CardRank.Ace) : floor1;
                ceil2 = deck.ToRankIndex(CardRank.King);
            }

            var isPair = IsPair(value);
            var isSuit = IsSuit(value);
            var isOffSuit = IsOffSuit(value);

            if (isPair)
            {
                floor2 = floor1;
                ceil2 = ceil1;
            }

            // If a range like "A4s+" was specified, increment only the
            // bottom card ie, "A4s, A5s, A6s, ..., AQs, AKs
            var rank1Step = (plus > 0 || slice > 0) && floor1 == deck.ToRankIndex(CardRank.Ace) ? 0 : 1;

            for (int rank1 = floor1, rank2 = floor2; rank1 <= ceil1 && rank2 <= ceil2; rank1 += rank1Step, ++rank2)
            {
                for (var suit1 = 0; suit1 != deck.NoOfSuits; ++suit1)
                {
                    var suit2Floor = isPair ? suit1 + 1 : (isSuit ? suit1 : 0);
                    var suit2Ceil = isSuit ? suit1 : deck.NoOfSuits - 1;

                    for (var suit2 = suit2Floor; suit2 <= suit2Ceil; ++suit2)
                    {
                        if (isOffSuit && suit1 == suit2)
                        {
                            continue;
                        }

                        var cardMask1 = deck.ToCardMask(deck.ToCardIndex(deck.ToRank(rank1), deck.ToSuit(suit1)));
                        var cardMask2 = deck.ToCardMask(deck.ToCardIndex(deck.ToRank(rank2), deck.ToSuit(suit2)));
                        var cardMask = cardMask1 | cardMask2;

                        if (!CardMask.IsAnySameCardSet(deadMask, cardMask) &&
                            !pocketsDistribution.PocketsCardMasks.Contains(cardMask))
                        {
                            pocketsDistribution.PocketsCardMasks.Add(cardMask);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Determines if this is a "pair" sort of hand such as "AA" or "QQ+" or "JJ-88".
        /// </summary>
        private static bool IsPair(string value)
        {
            if (value.Length < 2)
            {
                return false;
            }

            return value[0] == value[1];
        }

        /// <summary>
        ///     Determine if this is an "off suit" sort of hand such as "A2s" or "T9s+" or "QJs-65s".
        /// </summary>
        private static bool IsSuit(string value)
        {
            if (value.Length < 3)
            {
                return false;
            }

            return value[2] == DefaultConnectorSuit;
        }

        /// <summary>
        ///     Determine if this is an "off suit" sort of hand such as "A2o" or "T9o+" or "QJo-65o".
        /// </summary>
        private static bool IsOffSuit(string value)
        {
            if (value.Length < 3)
            {
                return false;
            }

            return value[2] == DefaultConnectorOffSuit;
        }
    }
}
