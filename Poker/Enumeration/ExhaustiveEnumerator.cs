using System;
using System.Collections;
using System.Collections.Generic;
using Poker.Deck;

namespace Poker.Enumeration
{
    /// <summary>
    ///     Exhaustively enumerates all possible card outcomes.
    /// </summary>
    public sealed class ExhaustiveEnumerator : IEnumerator<CardMask>
    {
        private const int DefaultCardsToEnumerate = 2;
        private const int MinCardsToEnumerate = 1;
        private const int MaxCardsToEnumerate = 9;

        private CardMask[] _cardMask;

        private readonly int _noOfCardsInDeck;
        private readonly int _cardsToEnumerate;
        private readonly CardMask _deadMask;
        private readonly Func<int, CardMask> _toCardMask;
        private bool _disposed;
        private bool _isDeadEnd;
        private int _currentLevel;
        private int[] _index;

        public ExhaustiveEnumerator(IDeck deck,
            int cardsToEnumerate,
            CardMask deadMask)
        {
            if (deck == null)
            {
                throw new ArgumentNullException(nameof(deck));
            }

            _noOfCardsInDeck = deck.NoOfCards;
            _cardsToEnumerate = cardsToEnumerate < MinCardsToEnumerate || cardsToEnumerate > MaxCardsToEnumerate
                ? DefaultCardsToEnumerate
                : cardsToEnumerate;
            _deadMask = deadMask;
            _toCardMask = deck.ToCardMask;

            Reset();
        }

        /// <summary>
        ///     Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>
        ///     The element in the collection at the current position of the enumerator.
        /// </value>
        public CardMask Current { get; private set; }

        /// <summary>
        ///     Gets the current element in the collection.
        /// </summary>
        /// <value>
        ///     The current element in the collection.
        /// </value>
        object IEnumerator.Current => Current;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the enumerator was successfully advanced to the next element;
        ///     <c>false</c> if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (_isDeadEnd)
            {
                return false;
            }

            var isEnd = false;
            do
            {
                if (_index[_currentLevel] <= 0)
                {
                    if (_currentLevel == 0)
                    {
                        _isDeadEnd = true;
                        continue;
                    }

                    --_currentLevel;
                    if (_index[_currentLevel] <= 0)
                    {
                        continue;
                    }
                }

                --_index[_currentLevel];
                var tempCardMask = _toCardMask(_index[_currentLevel]);

                if (CardMask.IsAnySameCardSet(_deadMask, tempCardMask))
                {
                    continue;
                }

                _cardMask[_currentLevel] = _currentLevel == 0
                    ? tempCardMask
                    : _cardMask[_currentLevel - 1] | tempCardMask;

                ++_currentLevel;
                isEnd = _currentLevel == _cardsToEnumerate;

                if (!isEnd)
                {
                    _index[_currentLevel] = _index[_currentLevel - 1];
                }
            } while (!isEnd && !_isDeadEnd);

            --_currentLevel;

            Current = _isDeadEnd ? CardMask.Empty : _cardMask[_currentLevel];

            return !_isDeadEnd;
        }

        /// <summary>
        ///     Sets the enumerator to its initial position, which is before the first element in the
        ///     collection.
        /// </summary>
        public void Reset()
        {
            Current = CardMask.Empty;
            _isDeadEnd = false;
            _currentLevel = 0;
            _index = new int[_cardsToEnumerate];
            _cardMask = new CardMask[_cardsToEnumerate];
            _index[0] = _noOfCardsInDeck;
        }

        ~ExhaustiveEnumerator()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        ///     Whether to dispose all managed resources as well.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed resources.
                _index = null;
                _cardMask = null;
            }

            // Dispose un-managed resources.
            _disposed = true;
        }
    }
}
