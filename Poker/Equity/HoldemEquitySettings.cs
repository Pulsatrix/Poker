using Poker.Deck;
using Poker.Evaluation;

namespace Poker.Equity
{
    public class HoldemEquitySettings : IEquitySettings
    {
        private static readonly IEvaluator StandardEvaluator = new StandardEvaluator();

        private const int NoOfCardsToEvaluate = 7;

        public void Enumerate()
        {
            
        }

        public HandValue Evaluate(CardMask boardCardMask, CardMask enumeratedCardMask, CardMask pocketCardMask)
        {
            var finalboardCardMask = boardCardMask | enumeratedCardMask;
            var finalHandMask = finalboardCardMask | pocketCardMask;
            var handValue = HandValue.Empty;
            handValue.HighValue = StandardEvaluator.Evaluate(finalHandMask, NoOfCardsToEvaluate);
            handValue.LowValue = HandValue.NothingLow;
            return handValue;
        }
    }
}
