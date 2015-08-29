using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Poker.Deck;

namespace Poker.Equity
{
    public static class EquityManager
    {
        public static void EnumerateAndEvaluate(ICalculator calculator,
            string boardSet,
            string deadSet,
            IEnumerable<string> pocketsSet)
        {
            if (calculator == null)
            {
                throw new ArgumentNullException(nameof(calculator));
            }

            var deck = calculator.Deck;
            var boardCardMask = deck.Parse(boardSet);
            var deadCardMask = deck.Parse(deadSet);

            var pocketsCardMasks = new Collection<CardMask>();
            foreach (var pocketsCardMask in pocketsSet.Select(pockets => deck.Parse(pockets)))
            {
                pocketsCardMasks.Add(pocketsCardMask);
            }

            EnumerateAndEvaluate(calculator, boardCardMask, deadCardMask, pocketsCardMasks);
        }

        public static void EnumerateAndEvaluate(ICalculator calculator,
            CardMask boardCardMask,
            CardMask deadCardMask,
            IEnumerable<CardMask> pocketsCardMasks)
        {
            if (calculator == null)
            {
                throw new ArgumentNullException(nameof(calculator));
            }

            var pockets = pocketsCardMasks.ToList();
            deadCardMask = CombineCardMasks(boardCardMask, deadCardMask, pockets);

            var enumerator = calculator.GetEnumerator(boardCardMask, deadCardMask);
            var isAvailable = enumerator.MoveNext();

            if (isAvailable)
            {
                do
                {
                    var enumeratedCardMask = enumerator.Current;
                    Evaluate(calculator, boardCardMask, enumeratedCardMask, pockets);
                } while (enumerator.MoveNext());
            }
            else
            {
                Evaluate(calculator, boardCardMask, CardMask.Empty, pockets);
            }
        }

        private static CardMask CombineCardMasks(CardMask boardCardMask,
            CardMask deadCardMask,
            IEnumerable<CardMask> pocketsCardMasks)
        {
            deadCardMask |= boardCardMask;
            deadCardMask = pocketsCardMasks.Aggregate(deadCardMask,
                (current, pocketsCardMask) => current | pocketsCardMask);
            return deadCardMask;
        }

        private static void Evaluate(ICalculator calculator,
            CardMask boardCardMask,
            CardMask enumeratedCardMask,
            IEnumerable<CardMask> pocketsCardMasks)
        {
            foreach (var pocketsCardMask in pocketsCardMasks)
            {
                calculator.Evaluate(boardCardMask, enumeratedCardMask, pocketsCardMask);
            }
        }
    }
}
