using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Poker.Deck
{
    public sealed class PocketsDistribution : IFormattable
    {
        public PocketsDistribution(string pocketsDistributionSets)
        {
            PocketsDistributionSets = pocketsDistributionSets;
        }

        public string PocketsDistributionSets { get; }

        public ICollection<CardMask> PocketsCardMasks { get; } = new Collection<CardMask>();

        public override string ToString() => ToString(null, CultureInfo.InvariantCulture);

        public string ToString(string format, IFormatProvider formatProvider)
            =>
                string.Format(CultureInfo.CurrentCulture,
                    "Cards: {0}, Pockets: {1}",
                    PocketsDistributionSets,
                    PocketsCardMasks.Count);
    }
}