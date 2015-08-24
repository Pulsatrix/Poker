using System;
using System.Collections;
using System.Collections.Generic;
using Poker.Deck;

namespace Poker.Enumeration
{
    public sealed class RandomEnumerator : IEnumerator<CardMask>
    {
        private const long DefaultRandomTrials = 1L;
        private const int DefaultCardsToEnumerate = 2;
        private const int MinCardsToEnumerate = 1;
        private const int MaxCardsToEnumerate = 9;
        private readonly int _cardsToEnumerate;
        private readonly CardMask _deadMask;

        private readonly int _noOfCardsInDeck;

        private readonly Random _random = new Random();
        private readonly Func<int, CardMask> _toCardMask;
        private readonly long _totalTrials;
        private long _currentTrial;
        private bool _disposed;
        private bool _isDeadEnd;

        public RandomEnumerator(int noOfCardsInDeck,
            int cardsToEnumerate,
            CardMask deadMask,
            Func<int, CardMask> toCardMask)
        {
            _noOfCardsInDeck = noOfCardsInDeck;
            _cardsToEnumerate = cardsToEnumerate < MinCardsToEnumerate || cardsToEnumerate > MaxCardsToEnumerate
                ? DefaultCardsToEnumerate
                : cardsToEnumerate;
            _deadMask = deadMask;
            _toCardMask = toCardMask;
            _totalTrials = DefaultRandomTrials;

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

            ++_currentTrial;
            if (_currentTrial == _totalTrials)
            {
                _isDeadEnd = true;
            }

            Current = CardMask.Empty;
            var currentDeadMask = _deadMask;
            for (var i = 0; i != _cardsToEnumerate; ++i)
            {
                CardMask tempCardMask;
                do
                {
                    var randomIndex = _random.Next(_noOfCardsInDeck);
                    tempCardMask = _toCardMask(randomIndex);
                } while (CardMask.IsAnySameCardSet(currentDeadMask, tempCardMask));

                Current |= tempCardMask;
                currentDeadMask |= tempCardMask;
            }

            return true;
        }

        /// <summary>
        ///     Sets the enumerator to its initial position, which is before the first element in the
        ///     collection.
        /// </summary>
        public void Reset()
        {
            Current = CardMask.Empty;
            _isDeadEnd = false;
            _currentTrial = 0;
        }

        ~RandomEnumerator()
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
            }

            // Dispose un-managed resources.
            _disposed = true;
        }
    }
}
