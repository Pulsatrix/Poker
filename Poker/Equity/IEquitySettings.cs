using Poker.Deck;
using Poker.Evaluation;

namespace Poker.Equity
{
    public interface IEquitySettings
    {
        HandValue Evaluate(CardMask boardCardMask, CardMask enumeratedCardMask, CardMask pocketCardMask);
    }
}
