using System;
using System.Collections;
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
        private const char DefaultPocketsDistributionConcatenatedSetDelimiter = '|';
        private const char DefaultCardSetDelimiter = ',';
        private const char DefaultSlice = '-';
        private const char DefaultPlus = '+';
        private const char DefaultConnectorSuit = 's';
        private const char DefaultConnectorOffSuit = 'o';
        private const string DefaultRandomSet = "XxXx";

        public static IEnumerable<EvaluationResult> EnumerateAndEvaluate(ICalculator calculator,
            string boardSet,
            string deadSet,
            string pocketsDistributionConcatenatedSets)
        {
            if (calculator == null)
            {
                throw new ArgumentNullException(nameof(calculator));
            }

            var deck = calculator.Deck;
            var boardCardMask = deck.ParseCards(boardSet);
            var deadCardMask = deck.ParseCards(deadSet);

            var pocketsDistributions = ParsePocketsDistributions(deck,
                boardCardMask,
                deadCardMask,
                pocketsDistributionConcatenatedSets);

            var evaluationResults = EnumerateAndEvaluateDistribution(calculator,
                boardCardMask,
                deadCardMask,
                pocketsDistributions);
            return evaluationResults;
        }

        public static IEnumerable<EvaluationResult> EnumerateAndEvaluateDistribution(ICalculator calculator,
            CardMask boardCardMask,
            CardMask deadCardMask,
            IEnumerable<PocketsDistribution> pocketsDistributions)
        {
            if (calculator == null)
            {
                throw new ArgumentNullException(nameof(calculator));
            }

            var pockets = pocketsDistributions.ToList();
            var evaluationResults = InitEvaluationResults(pockets.Count);

            EnumerateAndEvaluateDistribution(calculator, boardCardMask, deadCardMask, pockets, evaluationResults);

            return evaluationResults;
        }

        private static void EnumerateAndEvaluateDistribution(ICalculator calculator,
            CardMask boardCardMask,
            CardMask deadCardMask,
            IList<PocketsDistribution> pocketsDistributions,
            IList<EvaluationResult> evaluationResults)
        {
            var pocketsDistributionCollection = new PocketsDistributionCollection(calculator.EnumerationType,
                pocketsDistributions,
                deadCardMask);
            var trialCount = 0L;
            var localState = InitLocalState(calculator, boardCardMask, deadCardMask, pocketsDistributions.Count);
            localState = pocketsDistributionCollection.Aggregate(localState,
                (current, pocketsCollection) => EnumerateAndEvaluate(pocketsCollection, current));
            UpdateEvaluationResults(localState, evaluationResults, ref trialCount);
            CalculateProbability(trialCount, evaluationResults);
        }

        public static IEnumerable<EvaluationResult> EnumerateAndEvaluate(ICalculator calculator,
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

            var evaluationResults = EnumerateAndEvaluate(calculator, boardCardMask, deadCardMask, pocketsCardMasks);
            return evaluationResults;
        }

        public static IEnumerable<EvaluationResult> EnumerateAndEvaluate(ICalculator calculator,
            CardMask boardCardMask,
            CardMask deadCardMask,
            IEnumerable<CardMask> pocketsCardMasks)
        {
            if (calculator == null)
            {
                throw new ArgumentNullException(nameof(calculator));
            }

            var pockets = pocketsCardMasks.ToList();
            var evaluationResults = InitEvaluationResults(pockets.Count);

            EnumerateAndEvaluate(calculator, boardCardMask, deadCardMask, pockets, evaluationResults);

            return evaluationResults;
        }

        private static void EnumerateAndEvaluate(ICalculator calculator,
            CardMask boardCardMask,
            CardMask deadCardMask,
            ICollection<CardMask> pockets,
            IList<EvaluationResult> evaluationResults)
        {
            var trialCount = 0L;
            var localState = InitLocalState(calculator, boardCardMask, deadCardMask, pockets.Count);
            localState = EnumerateAndEvaluate(pockets, localState);
            UpdateEvaluationResults(localState, evaluationResults, ref trialCount);
            CalculateProbability(trialCount, evaluationResults);
        }

        private static LocalState InitLocalState(ICalculator calculator,
            CardMask boardCardMask,
            CardMask deadCardMask,
            int playersCount)
        {
            var localState = new LocalState
            {
                Calculator = calculator,
                BoardCardMask = boardCardMask,
                DeadCardMask = deadCardMask,
                PlayersCount = playersCount,
                EvaluationResults = InitEvaluationResults(playersCount),
                TrialCount = 0
            };
            return localState;
        }

        private static LocalState EnumerateAndEvaluate(IEnumerable<CardMask> pockets, LocalState localState)
        {
            var calculator = localState.Calculator;
            if (calculator.EnumerationType == EnumerationType.Undefined)
            {
                calculator.EnumerationType = EnumerationType.Exhaustive;
            }

            var deadCardMask = CombineCardMasks(localState.BoardCardMask, localState.DeadCardMask, pockets);
            var deckEnumerator = calculator.GetEnumerator(localState.BoardCardMask, deadCardMask);
            var isAvailable = deckEnumerator.MoveNext();

            if (isAvailable)
            {
                do
                {
                    var enumeratedCardMask = deckEnumerator.Current;
                    Evaluate(calculator,
                        localState.BoardCardMask,
                        enumeratedCardMask,
                        pockets,
                        localState.EvaluationResults);
                    ++localState.TrialCount;
                } while (deckEnumerator.MoveNext());
            }
            else
            {
                Evaluate(calculator, localState.BoardCardMask, CardMask.Empty, pockets, localState.EvaluationResults);
                ++localState.TrialCount;
            }

            return localState;
        }

        private static void UpdateEvaluationResults(LocalState localState,
            IList<EvaluationResult> evaluationResults,
            ref long trialCount)
        {
            var thisLock = new object();

            lock (thisLock)
            {
                trialCount += localState.TrialCount;
                for (var i = 0; i != localState.PlayersCount; ++i)
                {
                    evaluationResults[i].HighWinCount += localState.EvaluationResults[i].HighWinCount;
                    evaluationResults[i].HighTieCount += localState.EvaluationResults[i].HighTieCount;
                    evaluationResults[i].HighLoseCount += localState.EvaluationResults[i].HighLoseCount;
                    evaluationResults[i].LowWinCount += localState.EvaluationResults[i].LowWinCount;
                    evaluationResults[i].LowTieCount += localState.EvaluationResults[i].LowTieCount;
                    evaluationResults[i].LowLoseCount += localState.EvaluationResults[i].LowLoseCount;
                    evaluationResults[i].ScoopCount += localState.EvaluationResults[i].ScoopCount;
                    evaluationResults[i].ExpectedValue += localState.EvaluationResults[i].ExpectedValue;
                }
            }
        }

        private static IList<EvaluationResult> InitEvaluationResults(int playersCount)
        {
            var evaluationResults = new EvaluationResult[playersCount];
            for (var i = 0; i != playersCount; ++i)
            {
                evaluationResults[i] = new EvaluationResult();
            }

            return evaluationResults;
        }

        private static void Evaluate(ICalculator calculator,
            CardMask boardCardMask,
            CardMask enumeratedCardMask,
            IEnumerable<CardMask> pocketsCardMasks,
            IList<EvaluationResult> evaluationResults)
        {
            var bestHandValue = HandValue.Nothing;
            var highShare = 0;
            var lowShare = 0;
            var index = 0;

            foreach (var handValue in
                pocketsCardMasks.Select(
                    pocketsCardMask => calculator.Evaluate(boardCardMask, enumeratedCardMask, pocketsCardMask)))
            {
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

                evaluationResults[index].HandValue = handValue;
                ++index;
            }

            CalculateExpectedValue(bestHandValue, highShare, lowShare, evaluationResults);
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

        private static void CalculateProbability(long trialCount, IList<EvaluationResult> evaluationResults)
        {
            foreach (var evaluationResult in evaluationResults)
            {
                evaluationResult.Probability = (evaluationResult.ExpectedValue/trialCount)*100.0F;
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

        public static IEnumerable<PocketsDistribution> ParsePocketsDistributions(IDeck deck,
            CardMask boardCardMask,
            CardMask deadCardMask,
            string pocketsDistributionConcatenatedSets)
        {
            if (deck == null)
            {
                throw new ArgumentNullException(nameof(deck));
            }

            if (pocketsDistributionConcatenatedSets == null)
            {
                throw new ArgumentNullException(nameof(pocketsDistributionConcatenatedSets));
            }

            var pocketsDistributionSetsCollection =
                pocketsDistributionConcatenatedSets.Split(DefaultPocketsDistributionConcatenatedSetDelimiter);

            var deadMask = deadCardMask | boardCardMask;
            var pocketsDistributions = new Collection<PocketsDistribution>();
            var unprocessedTokens = new Dictionary<int, string>();

            foreach (var token in pocketsDistributionSetsCollection)
            {
                CardMask cardMask;
                if (deck.TryParseCards(token, out cardMask))
                {
                    var specificDistribution = new PocketsDistribution(token);
                    specificDistribution.PocketsCardMasks.Add(cardMask);
                    pocketsDistributions.Add(specificDistribution);
                    deadMask |= cardMask;
                }
                else
                {
                    pocketsDistributions.Add(null);
                    unprocessedTokens.Add(pocketsDistributions.Count - 1, token);
                }
            }

            foreach (var token in unprocessedTokens)
            {
                pocketsDistributions[token.Key] = ParsePocketsDistribution(token.Value, deadMask, deck);
            }

            return pocketsDistributions;
        }

        public static PocketsDistribution ParsePocketsDistribution(string pocketsDistributionSets,
            CardMask deadCardMask,
            IDeck deck)
        {
            if (deck == null)
            {
                throw new ArgumentNullException(nameof(deck));
            }

            var pocketsDistribution = new PocketsDistribution(pocketsDistributionSets);

            if (string.IsNullOrEmpty(pocketsDistributionSets))
            {
                return pocketsDistribution;
            }

            var split = pocketsDistributionSets.Split(DefaultCardSetDelimiter);
            foreach (var token in split)
            {
                CardMask cardMask;
                if (string.IsNullOrEmpty(token) || token.Equals(DefaultRandomSet))
                {
                    AddRandomHand(deadCardMask, deck, pocketsDistribution);
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
                    ParseRange(token, deadCardMask, deck, pocketsDistribution);
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
        private static void AddRandomHand(CardMask deadCardMask, IDeck deck, PocketsDistribution pocketsDistribution)
        {
            using (var cardExhaustiveCollection = new ExhaustiveDeckEnumerator(deck, 2, deadCardMask))
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
        private static void ParseRange(string value,
            CardMask deadCardMask,
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
                ceil1 = (plus > 0) && (floor1 - floor2 <= 1) ? deck.ToRankIndex(CardRank.Ace) : floor1;
                ceil2 = (plus > 0) && (floor1 - floor2 <= 1) ? deck.ToRankIndex(CardRank.King) : floor1 - 1;
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
            var rank1Step = (plus > 0 || slice > 0) &&
                (floor1 == deck.ToRankIndex(CardRank.Ace) || (floor1 - floor2 > 1 && floor1 == ceil1))
                ? 0
                : 1;

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

                        if (!CardMask.IsAnySameCardSet(deadCardMask, cardMask) &&
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

        private sealed class LocalState
        {
            internal CardMask BoardCardMask;
            internal ICalculator Calculator;
            internal CardMask DeadCardMask;
            internal IList<EvaluationResult> EvaluationResults;
            internal int PlayersCount;
            internal long TrialCount;
        }

        private sealed class PocketsDistributionCollection : IEnumerable<IEnumerable<CardMask>>
        {
            private readonly IEnumerator<IEnumerable<CardMask>> _enumerator;

            internal PocketsDistributionCollection(EnumerationType enumerationType,
                IList<PocketsDistribution> pocketsDistributions,
                CardMask deadCardMask)
            {
                _enumerator = enumerationType == EnumerationType.Exhaustive
                    ? (IEnumerator<IEnumerable<CardMask>>)
                        new ExhaustivePocketsDistributionsEnumerator(pocketsDistributions, deadCardMask)
                    : new RandomPocketsDistributionsEnumerator(pocketsDistributions, deadCardMask);
            }

            public IEnumerator<IEnumerable<CardMask>> GetEnumerator() => _enumerator;

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
