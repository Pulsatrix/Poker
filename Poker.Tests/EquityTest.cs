using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poker.Equity;

namespace Poker.Tests
{
    [TestClass]
    public class EquityTest
    {
        private const float AllowedDelta = 1.0F;

        [TestMethod]
        public void EquityTest1()
        {
            var calculator = new HoldemCalculator();
            var evaluationResults = EquityManager.EnumerateAndEvaluate(calculator,
                "Ks7d4d",
                string.Empty,
                "AhKh|Td9s|QQ+,AQs+,AQo+|JJ-88|XxXx|XxXx|XxXx");

            float[] expected = { 36.7F, 5.49F, 19.3F, 6.2F, 10.73F, 10.64F, 10.58F };

            CheckResult(expected, evaluationResults);
        }

        private static void CheckResult(IList<float> expected, IEnumerable<EvaluationResult> actual)
        {
            var actuals = actual.ToList();
            for (var i = 0; i != actuals.Count; ++i)
            {
                Assert.AreNotEqual(actuals[i], float.NaN);
                Assert.AreEqual(expected[i], actuals[i].Probability, AllowedDelta);
            }
        }
    }
}
