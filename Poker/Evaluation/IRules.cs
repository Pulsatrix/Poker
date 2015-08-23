namespace Poker.Evaluation
{
    interface IRules
    {
        int NoOfHandTypes { get; }

        int ToHandTypeRank(HandType handType);
    }
}
