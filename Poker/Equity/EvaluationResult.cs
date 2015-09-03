using Poker.Evaluation;

namespace Poker.Equity
{
    public sealed class EvaluationResult
    {
        public HandValue HandValue { get; set; }

        public int HighWinCount { get; set; }

        public int HighTieCount { get; set; }

        public int HighLoseCount { get; set; }

        public int LowWinCount { get; set; }

        public int LowTieCount { get; set; }

        public int LowLoseCount { get; set; }

        public int ScoopCount { get; set; }

        public float ExpectedValue { get; set; }

        public float Probability { get; set; }
    }
}