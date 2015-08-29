using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Poker.Deck;
using Poker.Evaluation;

namespace Poker.Equity
{
    public static class EquityManager
    {
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
            var boardCardMask = deck.Parse(boardSet);
            var deadCardMask = deck.Parse(deadSet);

            var pocketsCardMasks = new Collection<CardMask>();
            foreach (var pocketsCardMask in pocketsSet.Select(pockets => deck.Parse(pockets)))
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
    }
}
