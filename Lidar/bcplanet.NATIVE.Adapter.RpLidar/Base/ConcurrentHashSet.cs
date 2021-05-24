// ***********************************************************************
// Assembly         : bcplanet.NATIVE.Adapter.RpLidar
// Author           : bcare
// Created          : 05-23-2021
//
// Last Modified By : bcare
// Last Modified On : 05-23-2021
// ***********************************************************************
// <copyright file="ConcurrentHashSet.cs" company="beCee Soft Art">
//     Copyright (c) André Spitzner. All rights reserved.
//    
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met :
//
// 1. Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
// this list of conditions and the following disclaimer in the documentation
// and /or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED.IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES(INCLUDING, BUT NOT LIMITED TO,
//PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
// OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT(INCLUDING NEGLIGENCE OR
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml.Serialization;

namespace bcplanet.NATIVE.Adapter.RpLidar.Base
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
        /// Initializes a new instance of the <see cref="ConcurrentHashSet{T}" /> class.
        /// </summary>
        /// <param name="hashSet">The hash set.</param>
        private ConcurrentHashSet(ConcurrentHashSet<T> hashSet)
         : base(hashSet)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentHashSet{T}" /> class.
        /// </summary>
        public ConcurrentHashSet()
            : base(new HashSet<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentHashSet{T}" /> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        public ConcurrentHashSet(IEnumerable<T> collection)
        : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentHashSet{T}" /> class.
        /// </summary>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing values in the set, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> implementation for the set type.</param>
        public ConcurrentHashSet(IEqualityComparer<T> comparer)
            : base(new HashSet<T>(comparer))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentHashSet{T}" /> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing values in the set, or <see langword="null" /> to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1" /> implementation for the set type.</param>
        public ConcurrentHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
            : base(new HashSet<T>(collection, comparer))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentHashSet{T}" /> class.
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
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
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
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
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
        /// Finalizes an instance of the <see cref="ConcurrentHashSet{T}" /> class.
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
        // ReSharper disable once InconsistentNaming
        public event NotifyCollectionChangedEventHandler CollectionChanged;
#pragma warning restore 67
    }

}
