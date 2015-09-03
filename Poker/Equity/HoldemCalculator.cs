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

        public EnumerationType EnumerationType { get; set; } = EnumerationType.Undefined;

        public IEnumerator<CardMask> GetEnumerator(CardMask boardCardMask, CardMask deadCardMask)
        {
            var noOfBoardCards = boardCardMask.NoOfCardsSet();
            var cardsToEnumerate = MaxNoOfBoardCards - noOfBoardCards;
            var deckEnumerator = EnumerationType == EnumerationType.Exhaustive
                ? (IEnumerator<CardMask>)new ExhaustiveDeckEnumerator(Deck, cardsToEnumerate, deadCardMask)
                : new RandomDeckEnumerator(Deck, cardsToEnumerate, deadCardMask);
            return deckEnumerator;
        }

        public HandValue Evaluate(CardMask boardCardMask, CardMask enumeratedCardMask, CardMask pocketsCardMask)
        {
            var finalboardCardMask = boardCardMask | enumeratedCardMask;
            var finalHandCardMask = finalboardCardMask | pocketsCardMask;
            var handValue = HandValue.Nothing;
            handValue.HighValue = StandardEvaluator.Evaluate(finalHandCardMask, NoOfCardsToEvaluate);
            return handValue;
        }
    }
}
