/* Copyright André Spitzner 1977 - 2020 */

using farif.Tools;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml.Serialization;

namespace farif.Base
{
    /// <summary>
    /// Class ConcurrentHashSet.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.IDisposable" />
    [Serializable]
    public class ConcurrentHashSet<T> : HashSet<T>, IDisposable
    {
        #region .Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentHashSet{T}"/> class.
        /// </summary>
        /// <param name="hashSet">The hash set.</param>
        private ConcurrentHashSet(ConcurrentHashSet<T> hashSet)
         : base(hashSet)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentHashSet{T}"/> class.
        /// </summary>
        public ConcurrentHashSet()
            : base(new HashSet<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentHashSet{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        public ConcurrentHashSet(IEnumerable<T> collection)
        : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentHashSet{T}"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing values in the set, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> implementation for the set type.</param>
        public ConcurrentHashSet(IEqualityComparer<T> comparer)
            : base(new HashSet<T>(comparer))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentHashSet{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing values in the set, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> implementation for the set type.</param>
        public ConcurrentHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
            : base(new HashSet<T>(collection, comparer))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentHashSet{T}"/> class.
        /// </summary>
        /// <param name="info">Ein <see cref="T:System.Runtime.Serialization.SerializationInfo" />-Objekt mit den zum Serialisieren des <see cref="T:System.Collections.Generic.HashSet`1" />-Objekts erforderlichen Informationen.</param>
        /// <param name="context">Eine <see cref="T:System.Runtime.Serialization.StreamingContext" />-Struktur, die die Quelle und das Ziel des serialisierten Streams enthält, der dem <see cref="T:System.Collections.Generic.HashSet`1" />-Objekt zugeordnet ist.</param>
        public ConcurrentHashSet(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion

        /// <summary>
        /// The lock
        /// </summary>
        [XmlIgnore]
        [field: NonSerialized]
        private ReaderWriterLockSlim _Lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        /// Called when [deserializing event].
        /// </summary>
        /// <param name="context">The context.</param>
        [OnDeserializing]
        public void OnDeserializingEvent(StreamingContext context)
        {
            _Lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        #region ICollection<T> 

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if added, <c>false</c> otherwise.</returns>
        public new bool Add(T item)
        {
            try
            {
                _Lock.EnterWriteLock();

                return base.Add(item);
            }
            finally
            {
                if (_Lock.IsWriteLockHeld)
                    _Lock.ExitWriteLock();
                Utility.BeginInvoke(() => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add)));
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public new void Clear()
        {
            _Lock.EnterWriteLock();
            try
            {
                base.Clear();
            }
            finally
            {
                if (_Lock.IsWriteLockHeld) _Lock.ExitWriteLock();
                Utility.Invoke(() => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)));
            }
        }

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.</returns>
        public new bool Contains(T item)
        {
            _Lock.EnterReadLock();
            try
            {
                return base.Contains(item);
            }
            finally
            {
                if (_Lock.IsReadLockHeld)
                    _Lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if removed, <c>false</c> otherwise.</returns>
        public new bool Remove(T item)
        {
            _Lock.EnterWriteLock();
            try
            {
                return base.Remove(item);
            }
            finally
            {
                if (_Lock.IsWriteLockHeld)
                    _Lock.ExitWriteLock();
                Utility.Invoke(() => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove)));
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public new int Count
        {
            get
            {
                _Lock.EnterReadLock();
                try
                {
                    return base.Count;
                }
                finally
                {
                    if (_Lock.IsReadLockHeld)
                        _Lock.ExitReadLock();
                }
            }
        }
        #endregion

        #region Dispose

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben durch, die mit der Freigabe, der Zurückgabe oder dem Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _Lock?.Dispose();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ConcurrentHashSet{T}"/> class.
        /// </summary>
        ~ConcurrentHashSet()
        {
            Dispose(false);
        }

        #endregion

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
#pragma warning disable 67
        public event NotifyCollectionChangedEventHandler CollectionChanged;
#pragma warning restore 67
    }

}
