using System;
using System.Globalization;
using System.Text;

namespace Poker.Deck
{
    internal static class CardFormat
    {
        private const string DefaultCardMaskFormat = "ia";

        internal static string Format(CardMask cardMask, string format, CardFormatInfo cardFormatInfo)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = DefaultCardMaskFormat;
            }

            return FormatCustomized(cardMask, format, cardFormatInfo);
        }

        private static string FormatCustomized(CardMask cardMask, string format, CardFormatInfo cardFormatInfo)
        {
            var deck = cardFormatInfo.Deck;
            var sb = new StringBuilder(128);
            var index1 = 0;
            var isOnePatternAlreadyProcessed = false;
            while (index1 < format.Length)
            {
                var num1 = 1;
                var patternChar = format[index1];

                if (patternChar == 'n')
                {
                    if (isOnePatternAlreadyProcessed)
                    {
                        sb.Append(cardFormatInfo.PatternSeparator);
                    }

                    sb.Append(((long) cardMask).ToString(CultureInfo.CurrentCulture));
                    isOnePatternAlreadyProcessed = true;
                }

                if (patternChar == 'i' || patternChar == 'a' || patternChar == 'c' || patternChar == 'r' ||
                    patternChar == 'R' || patternChar == 's' || patternChar == 'S')
                {
                    var patternChar2 = ParseNextChar(format, index1);
                    if (((patternChar == 'r' && patternChar2 == 's') || (patternChar == 's' && patternChar2 == 'r')) ||
                        ((patternChar == 'R' && patternChar2 == 'S') || (patternChar == 'S' && patternChar2 == 'R')))
                    {
                        num1 = 2;
                    }

                    FormatAllCards(cardMask, cardFormatInfo, isOnePatternAlreadyProcessed, sb, deck, patternChar, num1);

                    isOnePatternAlreadyProcessed = true;
                }

                index1 += num1;
            }

            return sb.ToString();
        }

        private static void FormatAllCards(CardMask cardMask,
            CardFormatInfo cardFormatInfo,
            bool isOnePatternAlreadyProcessed,
            StringBuilder sb,
            IDeck deck,
            char patternChar,
            int num1)
        {
            var cardIndexes = cardFormatInfo.Deck.ToCardIndexes(cardMask);
            var i = 0;
            foreach (var cardIndex in cardIndexes)
            {
                if (i == 0 && isOnePatternAlreadyProcessed)
                {
                    sb.Append(cardFormatInfo.PatternSeparator);
                }

                var rankIndex = deck.ToRankIndex(cardIndex);
                var suitIndex = deck.ToSuitIndex(cardIndex);

                switch (patternChar)
                {
                    case 'i':
                        if (i != 0)
                        {
                            sb.Append(cardFormatInfo.CardIndexesSeparator);
                        }

                        sb.Append(cardIndex.ToString(CultureInfo.CurrentCulture));
                        break;
                    case 'a':
                        FormatAbbreviatedName(cardFormatInfo, i, sb, rankIndex, suitIndex);
                        break;
                    case 'c':
                        FormatName(cardFormatInfo, i, sb, rankIndex, suitIndex);
                        break;
                    case 'r':
                    case 's':
                        FormatAbbreviatedName2(cardFormatInfo, i, sb, patternChar, rankIndex, suitIndex, num1);

                        break;
                    case 'R':
                    case 'S':
                        FormatName2(cardFormatInfo, i, sb, patternChar, rankIndex, suitIndex, num1);
                        break;
                    default:
                        throw new ArgumentException(patternChar.ToString());
                }

                ++i;
            }
        }

        private static void FormatName2(CardFormatInfo cardFormatInfo,
            int i,
            StringBuilder sb,
            char patternChar,
            int rankIndex,
            int suitIndex,
            int num1)
        {
            if (i != 0)
            {
                sb.Append(cardFormatInfo.CardNamesSeparator);
            }

            sb.Append(patternChar == 'R'
                ? cardFormatInfo.GetRankName(rankIndex)
                : cardFormatInfo.GetSuitName(suitIndex,
                    (cardFormatInfo.FormatSettings & CardFormatSettings.UseGenitiveSuit) != CardFormatSettings.None));

            if (num1 == 2)
            {
                sb.Append(cardFormatInfo.CardNamePartsSeparator);
                sb.Append(patternChar == 'R'
                    ? cardFormatInfo.GetSuitName(suitIndex,
                        (cardFormatInfo.FormatSettings & CardFormatSettings.UseGenitiveSuit) != CardFormatSettings.None)
                    : cardFormatInfo.GetRankName(rankIndex));
            }
        }

        private static void FormatAbbreviatedName2(CardFormatInfo cardFormatInfo,
            int i,
            StringBuilder sb,
            char patternChar,
            int rankIndex,
            int suitIndex,
            int num1)
        {
            if (i != 0)
            {
                sb.Append(cardFormatInfo.CardAbbreviationsSeparator);
            }

            sb.Append(patternChar == 'r'
                ? cardFormatInfo.GetAbbreviatedRankName(rankIndex)
                : cardFormatInfo.GetAbbreviatedSuitName(suitIndex));

            if (num1 == 2)
            {
                sb.Append(cardFormatInfo.CardAbbreviationPartsSeparator);
                sb.Append(patternChar == 'r'
                    ? cardFormatInfo.GetAbbreviatedSuitName(suitIndex)
                    : cardFormatInfo.GetAbbreviatedRankName(rankIndex));
            }
        }

        private static void FormatName(CardFormatInfo cardFormatInfo, int i, StringBuilder sb, int rankIndex, int suitIndex)
        {
            if (i != 0)
            {
                sb.Append(cardFormatInfo.CardNamesSeparator);
            }

            sb.Append((cardFormatInfo.FormatSettings & CardFormatSettings.UseSuitFirst) == CardFormatSettings.None
                ? cardFormatInfo.GetRankName(rankIndex)
                : cardFormatInfo.GetSuitName(suitIndex,
                    (cardFormatInfo.FormatSettings & CardFormatSettings.UseGenitiveSuit) != CardFormatSettings.None));
            sb.Append(cardFormatInfo.CardNamePartsSeparator);
            sb.Append((cardFormatInfo.FormatSettings & CardFormatSettings.UseSuitFirst) == CardFormatSettings.None
                ? cardFormatInfo.GetSuitName(suitIndex,
                    (cardFormatInfo.FormatSettings & CardFormatSettings.UseGenitiveSuit) != CardFormatSettings.None)
                : cardFormatInfo.GetRankName(rankIndex));
        }

        private static void FormatAbbreviatedName(CardFormatInfo cardFormatInfo,
            int i,
            StringBuilder sb,
            int rankIndex,
            int suitIndex)
        {
            if (i != 0)
            {
                sb.Append(cardFormatInfo.CardAbbreviationsSeparator);
            }

            sb.Append((cardFormatInfo.FormatSettings & CardFormatSettings.UseSuitFirst) == CardFormatSettings.None
                ? cardFormatInfo.GetAbbreviatedRankName(rankIndex)
                : cardFormatInfo.GetAbbreviatedSuitName(suitIndex));
            sb.Append(cardFormatInfo.CardAbbreviationPartsSeparator);
            sb.Append((cardFormatInfo.FormatSettings & CardFormatSettings.UseSuitFirst) == CardFormatSettings.None
                ? cardFormatInfo.GetAbbreviatedSuitName(suitIndex)
                : cardFormatInfo.GetAbbreviatedRankName(rankIndex));
        }

        private static char ParseNextChar(string format, int pos)
            => pos >= format.Length - 1 ? char.MinValue : format[pos + 1];
    }
}
