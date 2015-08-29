using System.Collections.Generic;
using Poker.Deck;
using Poker.Enumeration;
using Poker.Evaluation;

namespace Poker.Equity
{
    public interface ICalculator
    {
        IDeck Deck { get; }

        EnumerationType EnumerationType { get; set; }

        IEnumerator<CardMask> GetEnumerator(CardMask boardCardMask, CardMask deadCardMask);

        HandValue Evaluate(CardMask boardCardMask, CardMask enumeratedCardMask, CardMask pocketsCardMask);
    }
}
