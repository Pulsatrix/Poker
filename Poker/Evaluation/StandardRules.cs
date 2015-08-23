using System.Diagnostics;

namespace Poker.Evaluation
{
    public class StandardRules : IRules
    {
        private const int HandTypeCount = 9;

        public StandardRules()
        {
            NoOfHandTypes = HandTypeCount;
        }

        public int NoOfHandTypes { get; }

        [DebuggerStepThrough]
        public int ToHandTypeRank(HandType handType)
        {
            int rank;

            switch (handType)
            {
                case HandType.NoPair:
                    rank = 0;
                    break;
                case HandType.OnePair:
                    rank = 1;
                    break;
                case HandType.TwoPair:
                    rank = 2;
                    break;
                case HandType.Trips:
                    rank = 3;
                    break;
                case HandType.Straight:
                    rank = 4;
                    break;
                case HandType.Flush:
                    rank = 5;
                    break;
                case HandType.FullHouse:
                    rank = 6;
                    break;
                case HandType.Quads:
                    rank = 7;
                    break;
                case HandType.StraightFlush:
                    rank = 8;
                    break;
                default:
                    rank = -1;
                    break;
            }

            return rank;
        }
    }
}