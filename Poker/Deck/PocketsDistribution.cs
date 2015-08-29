using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Poker.Deck
{
    public sealed class PocketsDistribution
    {
        public IEnumerable<CardMask> PocketsCardMasks { get; } = new Collection<CardMask>();
    }
}