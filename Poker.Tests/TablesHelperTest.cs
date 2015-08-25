using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poker.Deck;
using Poker.Evaluation;

namespace Poker.Tests
{
    [TestClass]
    public class TablesHelperTest
    {
        [TestMethod]
        public void GeneratedStandardDeckCardMaskTableShouldMatchActualOne()
        {
            var deck = new StandardDeck();

            var standardTable = DeckTablesHelper.GenerateStandardDeckCardMaskTable();

            for (var cardIndex = 0; cardIndex != deck.NoOfCards; ++cardIndex)
            {
                var generatedValue = standardTable[cardIndex];
                var actualValue = StandardDeck.CardMaskTable[cardIndex];
                Assert.AreEqual(generatedValue, actualValue);
            }
        }

        [TestMethod]
        public void GeneratedStandardEvaluatorBitsTableShouldMatchActualOne()
        {
            var bitsTable = EvaluatorTablesHelper.GenerateStandardEvaluatorBitsTable();

            for (var rankMaskIndex = 0; rankMaskIndex != StandardDeck.NoOfRankMasks; ++rankMaskIndex)
            {
                var generatedValue = bitsTable[rankMaskIndex];
                var actualValue = StandardEvaluator.BitsTable[rankMaskIndex];
                Assert.AreEqual(generatedValue, actualValue);
            }
        }

        [TestMethod]
        public void GeneratedStandardEvaluatorStraightTableShouldMatchActualOne()
        {
            var straightTable = EvaluatorTablesHelper.GenerateStandardEvaluatorStraightTable();

            for (var rankMaskIndex = 0; rankMaskIndex != StandardDeck.NoOfRankMasks; ++rankMaskIndex)
            {
                var generatedValue = straightTable[rankMaskIndex];
                var actualValue = StandardEvaluator.StraightTable[rankMaskIndex];
                Assert.AreEqual(generatedValue, actualValue);
            }
        }

        [TestMethod]
        public void GeneratedStandardEvaluatorTopCardTableShouldMatchActualOne()
        {
            var topCardTable = EvaluatorTablesHelper.GenerateStandardEvaluatorTopCardTable();

            for (var rankMaskIndex = 0; rankMaskIndex != StandardDeck.NoOfRankMasks; ++rankMaskIndex)
            {
                var generatedValue = topCardTable[rankMaskIndex];
                var actualValue = StandardEvaluator.TopCardTable[rankMaskIndex];
                Assert.AreEqual(generatedValue, actualValue);
            }
        }

        [TestMethod]
        public void GeneratedStandardEvaluatorTopFiveCardsTableShouldMatchActualOne()
        {
            var topFiveCardsTable = EvaluatorTablesHelper.GenerateStandardEvaluatorTopFiveCardsTable();

            for (var rankMaskIndex = 0; rankMaskIndex != StandardDeck.NoOfRankMasks; ++rankMaskIndex)
            {
                var generatedValue = topFiveCardsTable[rankMaskIndex];
                var actualValue = StandardEvaluator.TopFiveCardsTable[rankMaskIndex];
                Assert.AreEqual(generatedValue, actualValue);
            }
        }
    }
}
