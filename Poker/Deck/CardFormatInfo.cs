﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.Deck
{
    public sealed class CardFormatInfo : IFormatProvider
    {
        private const string DefaultPatternSeparator = " ";
        private const string DefaultCardIndexesSeparator = ",";
        private const string DefaultCardNamesSeparator = ",";
        private const string DefaultCardNamePartsSeparator = " ";
        private const string DefaultCardAbbreviationsSeparator = "";
        private const string DefaultCardAbbreviationPartsSeparator = "";

        private static volatile CardFormatInfo _defaultInstance;
        private IList<string> _abbreviatedRankNames;
        private IList<string> _abbreviatedSuitNames;

        private CardFormatSettings _formatSettings = CardFormatSettings.NotInitialized;
        private IList<string> _genitiveSuitNames;
        private IList<string> _rankNames;
        private IList<string> _suitNames;

        public CardFormatInfo() : this(StandardDeck.DefaultInstance)
        {
        }

        public CardFormatInfo(IDeck deck)
        {
            if (deck == null)
            {
                throw new ArgumentNullException(nameof(deck));
            }

            Deck = deck;
        }

        public IDeck Deck { get; private set; }

        public static CardFormatInfo CurrentInfo => DefaultInstance;

        public CardFormatSettings FormatSettings
        {
            get
            {
                if ((_formatSettings | CardFormatSettings.NotInitialized) != CardFormatSettings.NotInitialized)
                {
                    return _formatSettings;
                }

                _formatSettings = CardFormatSettings.None;
                _formatSettings = _formatSettings | (EqualStringArrays(GetSuitNames(), GetGenitiveSuitNames())
                    ? CardFormatSettings.None
                    : CardFormatSettings.UseGenitiveSuit);
                return _formatSettings;
            }

            set { _formatSettings = value; }
        }

        public IList<string> RankNames
        {
            get { return GetRankNames().ToList(); }
        }

        public IList<string> AbbreviatedRankNames
        {
            get { return GetAbbreviatedRankNames().ToList(); }
        }

        public IList<string> SuitNames
        {
            get { return GetSuitNames().ToList(); }
        }

        public IList<string> AbbreviatedSuitNames
        {
            get { return GetAbbreviatedSuitNames().ToList(); }
        }

        public IList<string> GenitiveSuitNames
        {
            get { return GetGenitiveSuitNames().ToList(); }
        }

        public string PatternSeparator { get; set; } = DefaultPatternSeparator;

        public string CardIndexesSeparator { get; set; } = DefaultCardIndexesSeparator;

        public string CardNamesSeparator { get; set; } = DefaultCardNamesSeparator;

        public string CardNamePartsSeparator { get; set; } = DefaultCardNamePartsSeparator;

        public string CardAbbreviationsSeparator { get; set; } = DefaultCardAbbreviationsSeparator;

        public string CardAbbreviationPartsSeparator { get; set; } = DefaultCardAbbreviationPartsSeparator;

        public static CardFormatInfo DefaultInstance => _defaultInstance ?? (_defaultInstance = new CardFormatInfo());

        public object GetFormat(Type formatType) => formatType == typeof (CardFormatInfo) ? this : null;

        public static CardFormatInfo GetInstance(IFormatProvider formatProvider)
        {
            var info = formatProvider as CardFormatInfo;
            if (info != null)
            {
                return info;
            }

            if (formatProvider != null)
            {
                info = formatProvider.GetFormat(typeof (CardFormatInfo)) as CardFormatInfo;
                if (info != null)
                {
                    return info;
                }
            }

            return CurrentInfo;
        }

        public string GetRankName(int rankIndex)
        {
            if (rankIndex < 0 || rankIndex >= Deck.NoOfRanks)
            {
                throw new ArgumentOutOfRangeException(nameof(rankIndex));
            }

            return GetRankNames()[rankIndex];
        }

        public string GetAbbreviatedRankName(int rankIndex)
        {
            if (rankIndex < 0 || rankIndex >= Deck.NoOfRanks)
            {
                throw new ArgumentOutOfRangeException(nameof(rankIndex));
            }

            return GetAbbreviatedRankNames()[rankIndex];
        }

        public string GetSuitName(int suitIndex, bool useGenitive)
        {
            if (suitIndex < 0 || suitIndex >= Deck.NoOfSuits)
            {
                throw new ArgumentOutOfRangeException(nameof(suitIndex));
            }

            return useGenitive ? GetGenitiveSuitNames()[suitIndex] : GetSuitNames()[suitIndex];
        }

        public string GetAbbreviatedSuitName(int suitIndex)
        {
            if (suitIndex < 0 || suitIndex >= Deck.NoOfSuits)
            {
                throw new ArgumentOutOfRangeException(nameof(suitIndex));
            }

            return GetAbbreviatedSuitNames()[suitIndex];
        }

        private IList<string> GetRankNames() => _rankNames ?? (_rankNames = Deck.RankNames);

        private IList<string> GetAbbreviatedRankNames()
            => _abbreviatedRankNames ?? (_abbreviatedRankNames = Deck.AbbreviatedRankNames);

        private IList<string> GetSuitNames() => _suitNames ?? (_suitNames = Deck.SuitNames);

        private IList<string> GetAbbreviatedSuitNames()
            => _abbreviatedSuitNames ?? (_abbreviatedSuitNames = Deck.AbbreviatedSuitNames);

        private IList<string> GetGenitiveSuitNames()
            => _genitiveSuitNames ?? (_genitiveSuitNames = Deck.GenitiveSuitNames);

        private static bool EqualStringArrays(IList<string> array1, IList<string> array2)
        {
            if (Equals(array1, array2))
            {
                return true;
            }

            if (array1.Count != array2.Count)
            {
                return false;
            }

            for (var index = 0; index != array1.Count; ++index)
            {
                if (!array1[index].Equals(array2[index]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
