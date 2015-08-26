using Poker.Deck;

namespace Poker.Evaluation
{
    interface IEvaluator
    {
        int Evaluate(CardMask cardMask, int noOfCardsToEvaluate);
    }
}
