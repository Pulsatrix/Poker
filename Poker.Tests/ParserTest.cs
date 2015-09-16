using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poker.Deck;
using Poker.Equity;

namespace Poker.Tests
{
    [TestClass]
    public sealed class ParserTest
    {
        // http://www.pokerstrategy.com/strategy/others/2244/1/
        [TestMethod]
        public void PocketsDistributionsParsingShouldMatchPredefinedResults1()
        {
            var calculator = new HoldemCalculator();
            var boardSet = string.Empty;
            var deadSet = string.Empty;
            const string pocketsDistributionConcatenatedSets = "AQ|AQs|AQo|AQ+|AQs+|AQo+|KJ|KJs|KJo|KJ+|KJs+|KJo+";
            int[] expected = {16, 4, 12, 32, 8, 24, 16, 4, 12, 32, 8, 24};

            var deck = calculator.Deck;
            var boardCardMask = deck.ParseCards(boardSet);
            var deadCardMask = deck.ParseCards(deadSet);

            var pocketsDistributions = EquityManager.ParsePocketsDistributions(deck,
                boardCardMask,
                deadCardMask,
                pocketsDistributionConcatenatedSets);

            CheckResult(pocketsDistributions, expected);
        }

        [TestMethod]
        public void PocketsDistributionsParsingShouldMatchPredefinedResults2()
        {
            var calculator = new HoldemCalculator();
            var boardSet = string.Empty;
            var deadSet = string.Empty;
            const string pocketsDistributionConcatenatedSets = "AK|AKs|AKo|AK+|AKs+|AKo+|KQ|KQs|KQo|KQ+|KQs+|KQo+";
            int[] expected = {16, 4, 12, 16, 4, 12, 16, 4, 12, 32, 8, 24};

            var deck = calculator.Deck;
            var boardCardMask = deck.ParseCards(boardSet);
            var deadCardMask = deck.ParseCards(deadSet);

            var pocketsDistributions = EquityManager.ParsePocketsDistributions(deck,
                boardCardMask,
                deadCardMask,
                pocketsDistributionConcatenatedSets);

            CheckResult(pocketsDistributions, expected);
        }

        [TestMethod]
        public void PocketsDistributionsParsingShouldMatchPredefinedResults3()
        {
            var calculator = new HoldemCalculator();
            var boardSet = string.Empty;
            var deadSet = string.Empty;
            const string pocketsDistributionConcatenatedSets = "AA|AAs|AAo|AA+|AAs+|AAo+|QQ|QQs|QQo|QQ+|QQs+|QQo+";
            int[] expected = {6, 0, 6, 6, 0, 6, 6, 0, 6, 18, 0, 18};

            var deck = calculator.Deck;
            var boardCardMask = deck.ParseCards(boardSet);
            var deadCardMask = deck.ParseCards(deadSet);

            var pocketsDistributions = EquityManager.ParsePocketsDistributions(deck,
                boardCardMask,
                deadCardMask,
                pocketsDistributionConcatenatedSets);

            CheckResult(pocketsDistributions, expected);
        }

        [TestMethod]
        public void PocketsDistributionsParsingShouldMatchPredefinedResults4()
        {
            var calculator = new HoldemCalculator();
            var boardSet = string.Empty;
            var deadSet = string.Empty;
            const string pocketsDistributionConcatenatedSets = "JJ-JJ|JJs-JJ|JJo-JJ|JJ-99|JJs-99|JJo-99|99-JJ";
            int[] expected = {6, 0, 6, 18, 0, 18, 0};

            var deck = calculator.Deck;
            var boardCardMask = deck.ParseCards(boardSet);
            var deadCardMask = deck.ParseCards(deadSet);

            var pocketsDistributions = EquityManager.ParsePocketsDistributions(deck,
                boardCardMask,
                deadCardMask,
                pocketsDistributionConcatenatedSets);

            CheckResult(pocketsDistributions, expected);
        }

        [TestMethod]
        public void PocketsDistributionsParsingShouldMatchPredefinedResults5()
        {
            var calculator = new HoldemCalculator();
            var boardSet = string.Empty;
            var deadSet = string.Empty;
            const string pocketsDistributionConcatenatedSets = "JT-JT|JTs-JT|JTo-JT|JT-98|JTs-98|JTo-98|98-JT";
            int[] expected = {16, 4, 12, 48, 12, 36, 0};

            var deck = calculator.Deck;
            var boardCardMask = deck.ParseCards(boardSet);
            var deadCardMask = deck.ParseCards(deadSet);

            var pocketsDistributions = EquityManager.ParsePocketsDistributions(deck,
                boardCardMask,
                deadCardMask,
                pocketsDistributionConcatenatedSets);

            CheckResult(pocketsDistributions, expected);
        }

        [TestMethod]
        public void PocketsDistributionsParsingShouldMatchPredefinedResults6()
        {
            var calculator = new HoldemCalculator();
            var boardSet = string.Empty;
            var deadSet = string.Empty;
            const string pocketsDistributionConcatenatedSets = "J9-J9|J9s-J9|J9o-J9|J9-97|J9s-97|J9o-97|97-J9";
            int[] expected = {16, 4, 12, 48, 12, 36, 0};

            var deck = calculator.Deck;
            var boardCardMask = deck.ParseCards(boardSet);
            var deadCardMask = deck.ParseCards(deadSet);

            var pocketsDistributions = EquityManager.ParsePocketsDistributions(deck,
                boardCardMask,
                deadCardMask,
                pocketsDistributionConcatenatedSets);

            CheckResult(pocketsDistributions, expected);
        }

        [TestMethod]
        public void PocketsDistributionsParsingShouldMatchPredefinedResults7()
        {
            var calculator = new HoldemCalculator();
            var boardSet = string.Empty;
            var deadSet = string.Empty;
            const string pocketsDistributionConcatenatedSets = "Q9-Q3|Q9s-Q3|Q9o-Q3";
            int[] expected = {112, 28, 84};

            var deck = calculator.Deck;
            var boardCardMask = deck.ParseCards(boardSet);
            var deadCardMask = deck.ParseCards(deadSet);

            var pocketsDistributions = EquityManager.ParsePocketsDistributions(deck,
                boardCardMask,
                deadCardMask,
                pocketsDistributionConcatenatedSets);

            CheckResult(pocketsDistributions, expected);
        }

        [TestMethod]
        public void PocketsDistributionsParsingShouldMatchPredefinedResults8()
        {
            var calculator = new HoldemCalculator();
            var boardSet = string.Empty;
            var deadSet = string.Empty;
            const string pocketsDistributionConcatenatedSets = "XxXx|88+,AJs+,KQs,AKo|QQ+,AQs+,AQo+";
            int[] expected = {1326, 70, 50};

            var deck = calculator.Deck;
            var boardCardMask = deck.ParseCards(boardSet);
            var deadCardMask = deck.ParseCards(deadSet);

            var pocketsDistributions = EquityManager.ParsePocketsDistributions(deck,
                boardCardMask,
                deadCardMask,
                pocketsDistributionConcatenatedSets);

            CheckResult(pocketsDistributions, expected);
        }

        [TestMethod]
        public void PocketsDistributionsParsingShouldMatchPredefinedResults10()
        {
            var calculator = new HoldemCalculator();
            const string boardSet = "Ks7d4d";
            var deadSet = string.Empty;
            const string pocketsDistributionConcatenatedSets = "AhKh|QQ+,AQs+,AQo+";
            int[] expected = {1, 28};

            var deck = calculator.Deck;
            var boardCardMask = deck.ParseCards(boardSet);
            var deadCardMask = deck.ParseCards(deadSet);

            var pocketsDistributions = EquityManager.ParsePocketsDistributions(deck,
                boardCardMask,
                deadCardMask,
                pocketsDistributionConcatenatedSets);

            CheckResult(pocketsDistributions, expected);
        }

        private static void CheckResult(IEnumerable<PocketsDistribution> pocketsDistributions, IList<int> expected)
        {
            var i = 0;
            foreach (var pocketsDistribution in pocketsDistributions)
            {
                Assert.AreEqual(expected[i], pocketsDistribution.PocketsCardMasks.Count);
                ++i;
            }
        }
    }
}
