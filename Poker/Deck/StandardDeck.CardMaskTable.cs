namespace Poker.Deck
{
    /// <summary>
    ///     Card mask table definition for <see cref="StandardDeck" />.
    /// </summary>
    public sealed partial class StandardDeck
    {
        /// <summary>
        ///     Represents card index to card mask lookup table. The card mask has only one bit set,
        ///     the bit corresponding to the card identified by the index.
        /// </summary>
        internal static readonly CardMask[] CardMaskTable =
        {
            (CardMask) 0x0001000000000000L, // Spades
            (CardMask) 0x0002000000000000L,
            (CardMask) 0x0004000000000000L,
            (CardMask) 0x0008000000000000L,
            (CardMask) 0x0010000000000000L,
            (CardMask) 0x0020000000000000L,
            (CardMask) 0x0040000000000000L,
            (CardMask) 0x0080000000000000L,
            (CardMask) 0x0100000000000000L,
            (CardMask) 0x0200000000000000L,
            (CardMask) 0x0400000000000000L,
            (CardMask) 0x0800000000000000L,
            (CardMask) 0x1000000000000000L,
            (CardMask) 0x0000000100000000L, // Clubs
            (CardMask) 0x0000000200000000L,
            (CardMask) 0x0000000400000000L,
            (CardMask) 0x0000000800000000L,
            (CardMask) 0x0000001000000000L,
            (CardMask) 0x0000002000000000L,
            (CardMask) 0x0000004000000000L,
            (CardMask) 0x0000008000000000L,
            (CardMask) 0x0000010000000000L,
            (CardMask) 0x0000020000000000L,
            (CardMask) 0x0000040000000000L,
            (CardMask) 0x0000080000000000L,
            (CardMask) 0x0000100000000000L,
            (CardMask) 0x0000000000010000L, // Diamonds
            (CardMask) 0x0000000000020000L,
            (CardMask) 0x0000000000040000L,
            (CardMask) 0x0000000000080000L,
            (CardMask) 0x0000000000100000L,
            (CardMask) 0x0000000000200000L,
            (CardMask) 0x0000000000400000L,
            (CardMask) 0x0000000000800000L,
            (CardMask) 0x0000000001000000L,
            (CardMask) 0x0000000002000000L,
            (CardMask) 0x0000000004000000L,
            (CardMask) 0x0000000008000000L,
            (CardMask) 0x0000000010000000L,
            (CardMask) 0x0000000000000001L, // Hearts
            (CardMask) 0x0000000000000002L,
            (CardMask) 0x0000000000000004L,
            (CardMask) 0x0000000000000008L,
            (CardMask) 0x0000000000000010L,
            (CardMask) 0x0000000000000020L,
            (CardMask) 0x0000000000000040L,
            (CardMask) 0x0000000000000080L,
            (CardMask) 0x0000000000000100L,
            (CardMask) 0x0000000000000200L,
            (CardMask) 0x0000000000000400L,
            (CardMask) 0x0000000000000800L,
            (CardMask) 0x0000000000001000L
        };
    }
}
