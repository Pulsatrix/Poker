using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Poker.Deck;

namespace Poker.Enumeration
{
    public sealed class RandomPocketsDistributionsEnumerator : IEnumerator<IEnumerable<CardMask>>
    {
        private const long DefaultRandomTrials = 1000000L;

        private static readonly Random Random = new Random();

        private readonly IList<CardMask> _current;
        private readonly CardMask _deadCardMask;
        private readonly int[] _handsCount;
        private readonly IList<CardMask>[] _index;
        private readonly int _noOfPlayers;
        private readonly long _noOfTrials;

        private readonly IList<PocketsDistribution> _pocketsDistributions;

        private long _currentTrial;
        private bool _disposed;
        private int _firstPlayerIndex;
        private bool _isDeadEnd;

        public RandomPocketsDistributionsEnumerator(IList<PocketsDistribution> pocketsDistributions,
            CardMask deadCardMask)
        {
            if (pocketsDistributions == null)
            {
                throw new ArgumentNullException(nameof(pocketsDistributions));
            }

            _pocketsDistributions = pocketsDistributions;
            _noOfPlayers = _pocketsDistributions.Count;
            _deadCardMask = deadCardMask;

            if (_noOfTrials < 1L)
            {
                _noOfTrials = DefaultRandomTrials;
            }

            _current = new CardMask[_noOfPlayers];
            _handsCount = new int[_noOfPlayers];
            _index = new IList<CardMask>[_noOfPlayers];

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

            bool isCollision;
            do
            {
                ++_currentTrial;
                if (_currentTrial == _noOfTrials)
                {
                    _isDeadEnd = true;
                }

                isCollision = false;
                var currentDeadCardMask = _deadCardMask;
                var playerIndex = _firstPlayerIndex;
                do
                {
                    var currentCardMask = CardMask.Empty;
                    if (_handsCount[playerIndex] != 1)
                    {
                        for (var i = 0; i != 10; ++i)
                        {
                            var randomIndex = Random.Next(_handsCount[playerIndex]);
                            var tempCardMask = _index[playerIndex][randomIndex];

                            if (CardMask.IsAnySameCardSet(currentDeadCardMask, tempCardMask))
                            {
                                continue;
                            }

                            currentCardMask = tempCardMask;
                            break;
                        }

                        if (currentCardMask == CardMask.Empty)
                        {
                            isCollision = true;
                            break;
                        }

                        currentDeadCardMask |= currentCardMask;
                    }
                    else
                    {
                        currentCardMask = _index[playerIndex][0];
                    }

                    _current[playerIndex] = currentCardMask;

                    if (++playerIndex == _noOfPlayers)
                    {
                        playerIndex = 0;
                    }
                } while (playerIndex != _firstPlayerIndex);
            } while (isCollision && !_isDeadEnd);

            if (++_firstPlayerIndex == _noOfPlayers)
            {
                _firstPlayerIndex = 0;
            }

            return true;
        }

        /// <summary>
        ///     Sets the enumerator to its initial position, which is before the first element in the
        ///     collection.
        /// </summary>
        public void Reset()
        {
            _isDeadEnd = false;
            _currentTrial = 0;
            _firstPlayerIndex = Random.Next(_noOfPlayers);

            for (var i = 0; i != _noOfPlayers; ++i)
            {
                _current[i] = CardMask.Empty;
                _handsCount[i] = _pocketsDistributions[i].PocketsCardMasks.Count();
                _index[i] = _pocketsDistributions[i].PocketsCardMasks.ToList();
            }
        }

        ~RandomPocketsDistributionsEnumerator()
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
