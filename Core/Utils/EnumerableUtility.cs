using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Core.Utils
{
    /// <summary>
    /// primes = knownPrimes.Concat(computedPrimes).ToLazyList();
    /// </summary>
    public static class EnumerableUtility
    {
        public static IList<T> ToLazyList<T>(this IEnumerable<T> list)
        {
            return new LazyList<T>(list);
        }

        #region Nested type: LazyList

        private class LazyList<T> : IList<T>, IDisposable
        {
            private readonly List<T> _cached;
            private readonly IEnumerator<T> _enumerator;
            private bool _isFinished;

            public LazyList(IEnumerable<T> list)
            {
                _enumerator = list.GetEnumerator();
                _isFinished = false;
                _cached = new List<T>();
            }

            #region IDisposable Members

            public void Dispose()
            {
                _enumerator.Dispose();
                _isFinished = true;
            }

            #endregion

            #region IList<T> Members

            public T this[int index]
            {
                get
                {
                    if (index < 0)
                        throw new ArgumentOutOfRangeException("index");
                    while (_cached.Count <= index && !_isFinished)
                        GetNext();
                    return _cached[index];
                }
                set { throw new NotSupportedException(); }
            }

            public int Count
            {
                get
                {
                    Finish();
                    return _cached.Count;
                }
            }

            public IEnumerator<T> GetEnumerator()
            {
                int current = 0;
                while (current < _cached.Count || !_isFinished)
                {
                    if (current == _cached.Count)
                        GetNext();
                    if (current != _cached.Count)
                        yield return _cached[current];
                    current++;
                }
            }

            public int IndexOf(T item)
            {
                int result = _cached.IndexOf(item);
                while (result == -1 && !_isFinished)
                {
                    GetNext();
                    if (_cached.Last().Equals(item))
                        result = _cached.Count - 1;
                }
                return result;
            }

            public void Insert(int index, T item)
            {
                throw new NotSupportedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            public void Add(T item)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(T item)
            {
                return IndexOf(item) != -1;
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                foreach (T item in this)
                    array[arrayIndex++] = item;
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool Remove(T item)
            {
                throw new NotSupportedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            private void GetNext()
            {
                if (!_isFinished)
                {
                    if (_enumerator.MoveNext())
                    {
                        _cached.Add(_enumerator.Current);
                    }
                    else
                    {
                        _isFinished = true;
                        _enumerator.Dispose();
                    }
                }
            }

            private void Finish()
            {
                while (!_isFinished)
                    GetNext();
            }
        }

        #endregion
    }
}