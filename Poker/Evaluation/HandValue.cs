namespace Poker.Evaluation
{
    public static class HandValue
    {
        public const int Nothing = 0;
        public const int CardBitsWidth = 4;

        public const int HandTypeRankCardMask = 0x0F000000;
        public const int TopCardRankMask = 0x000F0000;
        public const int SecondCardRankMask = 0x0000F000;
        public const int ThirdCardRankMask = 0x00000F00;
        public const int FourthCardRankMask = 0x000000F0;
        public const int FifthCardRankMask = 0x0000000F;

        private const int HandTypeRankShift = 24;
        private const int TopCardRankShift = 16;
        private const int SecondCardRankShift = 12;
        private const int ThirdCardRankShift = 8;
        private const int FourthCardRankShift = 4;
        private const int FifthCardRankShift = 0;

        //public static readonly int NothingLow = FromHandTypeRank(HandTypeStandard.StraightFlush) +
        //    FromTopCard(DeckStandard.ToRankIndex(CardRank.Ace)) + 1;

        public static int FromHandTypeRank(int handTypeRank) => handTypeRank << HandTypeRankShift;

        public static int FromTopCardRank(int cardRank) => cardRank << TopCardRankShift;

        public static int FromSecondCardRank(int cardRank) => cardRank << SecondCardRankShift;

        public static int FromThirdCardRank(int cardRank) => cardRank << ThirdCardRankShift;

        public static int FromFourthCardRank(int cardRank) => cardRank << FourthCardRankShift;

        public static int FromFifthCardRank(int cardRank) => cardRank << FifthCardRankShift;
    }
}
