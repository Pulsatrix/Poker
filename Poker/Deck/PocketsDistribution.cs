using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Poker.Deck
{
    public sealed class PocketsDistribution
    {
        public ICollection<CardMask> PocketsCardMasks { get; } = new Collection<CardMask>();
    }
}