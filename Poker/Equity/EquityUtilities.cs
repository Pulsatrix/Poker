using System.Collections.Generic;

namespace Poker.Equity
{
    public static class EquityUtilities
    {
        public static IEnumerable<EvaluationResult> EnumerateAndEvaluate(ICalculator calculator,
            string boardSet,
            string deadSet,
            string pocketsDistributionConcatenatedSets)
        {
            var pocketsDistributionSets = cardSetParser.ParseTable(pocketsDistributionConcatenatedSets,
                boardSet,
                deadSet,
                formatProvider);

            return EquityManager.EnumerateAndEvaluateDistribution(calculator, boardSet, deadSet, pocketsDistributionSets);
        }
    }
}
