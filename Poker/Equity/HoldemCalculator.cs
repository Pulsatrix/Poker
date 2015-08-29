using System.Collections.Generic;
using Poker.Deck;
using Poker.Enumeration;
using Poker.Evaluation;

namespace Poker.Equity
{
    public class HoldemCalculator : ICalculator
    {
        private const int MaxNoOfBoardCards = 5;
        private const int NoOfCardsToEvaluate = 7;

        private static readonly IEvaluator StandardEvaluator = new StandardEvaluator();

        public IDeck Deck { get; } = new StandardDeck();

        public EnumerationType EnumerationType { get; set; } = EnumerationType.Random;

        public IEnumerator<CardMask> GetEnumerator(CardMask boardCardMask, CardMask deadCardMask)
        {
            var noOfBoardCards = boardCardMask.NoOfCardsSet();
            var cardsToEnumerate = MaxNoOfBoardCards - noOfBoardCards;
            var enumerator = EnumerationType == EnumerationType.Exhaustive
                ? (IEnumerator<CardMask>)new ExhaustiveEnumerator(Deck, cardsToEnumerate, deadCardMask)
                : new RandomEnumerator(Deck, cardsToEnumerate, deadCardMask);
            return enumerator;
        }

        public HandValue Evaluate(CardMask boardCardMask, CardMask enumeratedCardMask, CardMask pocketsCardMask)
        {
            var finalboardCardMask = boardCardMask | enumeratedCardMask;
            var finalHandMask = finalboardCardMask | pocketsCardMask;
            var handValue = HandValue.Empty;
            handValue.HighValue = StandardEvaluator.Evaluate(finalHandMask, NoOfCardsToEvaluate);
            handValue.LowValue = HandValue.NothingLow;
            return handValue;
        }
    }
}
