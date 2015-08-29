using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Poker.Deck;

namespace Poker.Enumeration
{
    public sealed class ExhaustivePocketsDistributionsEnumerator : IEnumerator<IEnumerable<CardMask>>
    {
        private readonly IList<CardMask> _current;
        private readonly CardMask[] _currentDeadMask;
        private readonly CardMask _deadCardMask;
        private readonly int[] _handsCount;
        private readonly IEnumerator<CardMask>[] _index;
        private readonly int _noOfPlayers;
        private readonly IList<PocketsDistribution> _pocketsDistributions;
        private bool _disposed;
        private bool _isDeadEnd;

        private int _playerIndex;

        public ExhaustivePocketsDistributionsEnumerator(IList<PocketsDistribution> pocketsDistributions,
            CardMask deadCardMask)
        {
            if (pocketsDistributions == null)
            {
                throw new ArgumentNullException(nameof(pocketsDistributions));
            }

            _pocketsDistributions = pocketsDistributions;
            _noOfPlayers = _pocketsDistributions.Count;
            _deadCardMask = deadCardMask;

            _current = new CardMask[_noOfPlayers];
            _handsCount = new int[_noOfPlayers];
            _index = new IEnumerator<CardMask>[_noOfPlayers];
            _currentDeadMask = new CardMask[_noOfPlayers];

            Reset();
        }

        /// <summary>
        ///     Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>
        ///     The element in the collection at the current position of the enumerator.
        /// </value>
        public IEnumerable<CardMask> Current => _current;

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

            var foundEnd = false;
            do
            {
                if (!_index[_playerIndex].MoveNext())
                {
                    _index[_playerIndex].Reset();

                    if (_playerIndex == 0)
                    {
                        _isDeadEnd = true;
                        continue;
                    }

                    --_playerIndex;
                    continue;
                }

                var tempCardMask = _index[_playerIndex].Current;

                if (
                    CardMask.IsAnySameCardSet(_playerIndex == 0 ? _deadCardMask : _currentDeadMask[_playerIndex - 1],
                        tempCardMask) && _handsCount[_playerIndex] != 1)
                {
                    continue;
                }

                _currentDeadMask[_playerIndex] = tempCardMask |
                    (_playerIndex == 0 ? _deadCardMask : _currentDeadMask[_playerIndex - 1]);

                _current[_playerIndex] = tempCardMask;

                ++_playerIndex;
                foundEnd = _playerIndex == _noOfPlayers;
            } while (!foundEnd && !_isDeadEnd);

            --_playerIndex;

            return !_isDeadEnd;
        }

        /// <summary>
        ///     Sets the enumerator to its initial position, which is before the first element in the
        ///     collection.
        /// </summary>
        public void Reset()
        {
            _isDeadEnd = false;
            _playerIndex = 0;

            for (var i = 0; i != _noOfPlayers; ++i)
            {
                _current[i] = CardMask.Empty;
                _handsCount[i] = _pocketsDistributions[i].PocketsCardMasks.Count();
                _index[i] = _pocketsDistributions[i].PocketsCardMasks.GetEnumerator();
            }
        }

        ~ExhaustivePocketsDistributionsEnumerator()
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
