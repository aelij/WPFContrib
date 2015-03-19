using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using Avalon.Internal.Utility;

namespace Avalon.Windows.Media.Animation
{
    /// <summary>Represents a collection of <see cref="KeyFrame{T}" /> objects. </summary>
    /// <typeparam name="T"></typeparam>
    public class KeyFrameCollection<T> : Freezable, IList<KeyFrame<T>>, IList
        where T : struct
    {
        #region Fields

        private static KeyFrameCollection<T> _emptyCollection;
        private List<KeyFrame<T>> _keyFrames;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="KeyFrameCollection{T}" /> class.</summary>
        public KeyFrameCollection()
        {
            _keyFrames = new List<KeyFrame<T>>(2);
        }

        #endregion

        #region Public Methods

        /// <summary>Adds a <see cref="KeyFrame{T}" /> to the end of the collection. </summary>
        /// <returns>The index at which the keyFrame was added.</returns>
        /// <param name="item">The <see cref="KeyFrame{T}" /> to add to the end of the collection.</param>
        public void Add(KeyFrame<T> item)
        {
            ArgumentValidator.NotNull(item, "item");

            WritePreamble();
            OnFreezablePropertyChanged(null, item);
            _keyFrames.Add(item);
            WritePostscript();
        }

        /// <summary>
        ///     Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            WritePreamble();
            if (_keyFrames.Count > 0)
            {
                foreach (var keyFrame in _keyFrames)
                {
                    OnFreezablePropertyChanged(keyFrame, null);
                }
                _keyFrames.Clear();
                WritePostscript();
            }
        }

        /// <summary>Gets a value that indicates whether the collection contains the specified <see cref="KeyFrame{T}" />. </summary>
        /// <returns>true if the collection contains keyFrame; otherwise, false.</returns>
        /// <param name="item">The <see cref="KeyFrame{T}" /> to locate in the collection.</param>
        public bool Contains(KeyFrame<T> item)
        {
            ReadPreamble();
            return _keyFrames.Contains(item);
        }

        /// <summary>Copies all of the <see cref="KeyFrame{T}" /> objects in a collection to a specified array. </summary>
        /// <param name="array">Identifies the array to which content is copied.</param>
        /// <param name="arrayIndex">Index position in the array to which the contents of the collection are copied.</param>
        public void CopyTo(KeyFrame<T>[] array, int arrayIndex)
        {
            ReadPreamble();
            _keyFrames.CopyTo(array, arrayIndex);
        }

        /// <summary> Returns an enumerator that can iterate through the collection. </summary>
        /// <returns>An enumerator that can iterate through the collection.</returns>
        public IEnumerator<KeyFrame<T>> GetEnumerator()
        {
            ReadPreamble();
            return _keyFrames.GetEnumerator();
        }

        /// <summary> Returns an enumerator that can iterate through the collection. </summary>
        /// <returns>An enumerator that can iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Searches for the specified <see cref="KeyFrame{T}" /> and returns the zero-based index of the first
        ///     occurrence within the entire collection.
        /// </summary>
        /// <returns>
        ///     The zero-based index of the first occurrence of keyFrame within the entire collection, if found; otherwise,
        ///     -1.
        /// </returns>
        /// <param name="item">The <see cref="KeyFrame{T}" /> to locate in the collection.</param>
        public int IndexOf(KeyFrame<T> item)
        {
            ReadPreamble();
            return _keyFrames.IndexOf(item);
        }

        /// <summary>Inserts a <see cref="KeyFrame{T}" /> into a specific location within the collection. </summary>
        /// <param name="item">The <see cref="KeyFrame{T}" /> object to insert in the collection.</param>
        /// <param name="index">The index position at which the <see cref="KeyFrame{T}" /> is inserted.</param>
        public void Insert(int index, KeyFrame<T> item)
        {
            ArgumentValidator.NotNull(item, "item");

            WritePreamble();
            OnFreezablePropertyChanged(null, item);
            _keyFrames.Insert(index, item);
            WritePostscript();
        }

        /// <summary>Removes a <see cref="KeyFrame{T}" /> object from the collection. </summary>
        /// <param name="item">Identifies the <see cref="KeyFrame{T}" /> to remove from the collection.</param>
        public bool Remove(KeyFrame<T> item)
        {
            WritePreamble();
            if (_keyFrames.Contains(item))
            {
                OnFreezablePropertyChanged(item, null);
                _keyFrames.Remove(item);
                WritePostscript();

                return true;
            }

            return false;
        }

        /// <summary>Removes the <see cref="KeyFrame{T}" /> at the specified index position from the collection. </summary>
        /// <param name="index">Index position of the <see cref="KeyFrame{T}" /> to be removed.</param>
        public void RemoveAt(int index)
        {
            WritePreamble();
            OnFreezablePropertyChanged(_keyFrames[index], null);
            _keyFrames.RemoveAt(index);
            WritePostscript();
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the number of key frames contained in the <see cref="KeyFrameCollection{T}" />.</summary>
        /// <returns>The number of key frames contained in the <see cref="KeyFrameCollection{T}" />.</returns>
        public int Count
        {
            get
            {
                ReadPreamble();
                return _keyFrames.Count;
            }
        }

        /// <summary> Gets an empty <see cref="KeyFrameCollection{T}" />.  </summary>
        /// <returns>An empty <see cref="KeyFrameCollection{T}" />.</returns>
        public static KeyFrameCollection<T> Empty
        {
            get
            {
                if (_emptyCollection == null)
                {
                    var collection = new KeyFrameCollection<T> { _keyFrames = new List<KeyFrame<T>>(0) };
                    collection.Freeze();
                    _emptyCollection = collection;
                }
                return _emptyCollection;
            }
        }

        /// <summary>Gets a value that indicates if the collection is frozen.</summary>
        /// <returns>true if the collection is frozen; otherwise, false.</returns>
        public bool IsFixedSize
        {
            get
            {
                ReadPreamble();
                return IsFrozen;
            }
        }

        /// <summary> Gets a value that indicates if the collection is read-only.</summary>
        /// <returns>true if the collection is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get
            {
                ReadPreamble();
                return IsFrozen;
            }
        }

        /// <summary>Gets a value that indicates whether access to the collection is synchronized (thread-safe). </summary>
        /// <returns>true if access to the collection is synchronized (thread-safe); otherwise, false.</returns>
        public bool IsSynchronized
        {
            get
            {
                ReadPreamble();
                if (!IsFrozen)
                {
                    return (Dispatcher != null);
                }
                return true;
            }
        }

        /// <summary>Gets or sets the <see cref="KeyFrame{T}" /> at the specified index position.  </summary>
        /// <returns>The <see cref="KeyFrame{T}" /> at the specified index.</returns>
        /// <param name="index">The zero-based index of the <see cref="KeyFrame{T}" /> to get or set.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     index is less than zero.-or-index is equal to or greater than
        ///     <see cref="KeyFrameCollection{T}.Count" />.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The attempt to modify the collection is invalid because the
        ///     collection is frozen (its <see cref="System.Windows.Freezable.IsFrozen" /> property is true).
        /// </exception>
        public KeyFrame<T> this[int index]
        {
            get
            {
                ReadPreamble();
                return _keyFrames[index];
            }
            set
            {
                ArgumentValidator.NotNull(value, "value");

                WritePreamble();
                if (!ReferenceEquals(value, _keyFrames[index]))
                {
                    OnFreezablePropertyChanged(_keyFrames[index], value);
                    _keyFrames[index] = value;
                    WritePostscript();
                }
            }
        }

        /// <summary> Gets an object that can be used to synchronize access to the collection. </summary>
        /// <returns>An object that can be used to synchronize access to the collection.</returns>
        public object SyncRoot
        {
            get
            {
                ReadPreamble();
                return ((ICollection)_keyFrames).SyncRoot;
            }
        }

        #endregion

        #region Freezable Methods

        /// <summary>
        ///     Creates a modifiable clone of this <see cref="KeyFrameCollection{T}" />, making deep copies of this object's
        ///     values. When copying dependency properties, this method copies resource references and data bindings (but they
        ///     might no longer resolve) but not animations or their current values.
        /// </summary>
        /// <returns>
        ///     A modifiable clone of the current object. The cloned object's <see cref="System.Windows.Freezable.IsFrozen" />
        ///     property will be false even if the source's <see cref="System.Windows.Freezable.IsFrozen" /> property was true.
        /// </returns>
        public new KeyFrameCollection<T> Clone()
        {
            return (KeyFrameCollection<T>)base.Clone();
        }

        /// <summary>
        ///     Makes this instance a deep copy of the specified <see cref="KeyFrameCollection{T}" />. When copying dependency
        ///     properties, this method copies resource references and data bindings (but they might no longer resolve) but not
        ///     animations or their current values.
        /// </summary>
        /// <param name="sourceFreezable">The <see cref="KeyFrameCollection{T}" /> to clone.</param>
        protected override void CloneCore(Freezable sourceFreezable)
        {
            KeyFrameCollection<T> collection = (KeyFrameCollection<T>)sourceFreezable;
            base.CloneCore(sourceFreezable);
            int count = collection._keyFrames.Count;
            _keyFrames = new List<KeyFrame<T>>(count);
            for (int i = 0; i < count; i++)
            {
                KeyFrame<T> keyFrame = (KeyFrame<T>)collection._keyFrames[i].Clone();
                _keyFrames.Add(keyFrame);
                OnFreezablePropertyChanged(null, keyFrame);
            }
        }

        /// <summary>
        ///     Makes this instance a modifiable deep copy of the specified <see cref="KeyFrameCollection{T}" /> using current
        ///     property values. Resource references, data bindings, and animations are not copied, but their current values are.
        /// </summary>
        /// <param name="sourceFreezable">The <see cref="KeyFrameCollection{T}" /> to clone.</param>
        protected override void CloneCurrentValueCore(Freezable sourceFreezable)
        {
            KeyFrameCollection<T> collection = (KeyFrameCollection<T>)sourceFreezable;
            base.CloneCurrentValueCore(sourceFreezable);
            int frameCount = collection._keyFrames.Count;
            _keyFrames = new List<KeyFrame<T>>(frameCount);
            for (int i = 0; i < frameCount; ++i)
            {
                KeyFrame<T> frame = (KeyFrame<T>)collection._keyFrames[i].CloneCurrentValue();
                _keyFrames.Add(frame);
                OnFreezablePropertyChanged(null, frame);
            }
        }

        /// <summary>Creates a new, frozen instance of <see cref="KeyFrameCollection{T}" />.</summary>
        /// <returns>A frozen instance of <see cref="KeyFrameCollection{T}" />.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new KeyFrameCollection<T>();
        }

        /// <summary>
        ///     Makes this instance of <see cref="KeyFrameCollection{T}" /> unmodifiable or determines whether it can be made
        ///     unmodifiable.
        /// </summary>
        /// <returns>
        ///     If isChecking is true, this method returns true if the specified <see cref="System.Windows.Freezable" /> can
        ///     be made unmodifiable, or false if it cannot be made unmodifiable. If isChecking is false, this method returns true
        ///     if the if the specified <see cref="System.Windows.Freezable" /> is now unmodifiable, or false if it cannot be made
        ///     unmodifiable, with the side effect of having made the actual change in frozen status to this object.
        /// </returns>
        /// <param name="isChecking">
        ///     true if the <see cref="System.Windows.Freezable" /> instance should actually freeze itself
        ///     when this method is called. false if the <see cref="System.Windows.Freezable" /> should simply return whether it
        ///     can be frozen.
        /// </param>
        protected override bool FreezeCore(bool isChecking)
        {
            bool shouldFreeze = base.FreezeCore(isChecking);
            for (int i = 0; (i < _keyFrames.Count) && shouldFreeze; ++i)
            {
                shouldFreeze &= Freeze(_keyFrames[i], isChecking);
            }
            return shouldFreeze;
        }

        /// <summary>Makes this instance a clone of the specified <see cref="KeyFrameCollection{T}" /> object. </summary>
        /// <param name="sourceFreezable">The <see cref="KeyFrameCollection{T}" /> object to clone.</param>
        protected override void GetAsFrozenCore(Freezable sourceFreezable)
        {
            KeyFrameCollection<T> source = (KeyFrameCollection<T>)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);
            int frameCount = source._keyFrames.Count;
            _keyFrames = new List<KeyFrame<T>>(frameCount);
            for (int i = 0; i < frameCount; ++i)
            {
                KeyFrame<T> keyFrame = (KeyFrame<T>)source._keyFrames[i].GetAsFrozen();
                _keyFrames.Add(keyFrame);
                OnFreezablePropertyChanged(null, keyFrame);
            }
        }

        /// <summary>
        ///     Makes this instance a modifiable deep copy of the specified <see cref="KeyFrameCollection{T}" /> using current
        ///     property values. Resource references, data bindings, and animations are not copied, but their current values are.
        /// </summary>
        /// <param name="sourceFreezable">The <see cref="KeyFrameCollection{T}" /> to clone.</param>
        protected override void GetCurrentValueAsFrozenCore(Freezable sourceFreezable)
        {
            KeyFrameCollection<T> collection = (KeyFrameCollection<T>)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);
            int frameCount = collection._keyFrames.Count;
            _keyFrames = new List<KeyFrame<T>>(frameCount);
            for (int i = 0; i < frameCount; ++i)
            {
                KeyFrame<T> frame = (KeyFrame<T>)collection._keyFrames[i].GetCurrentValueAsFrozen();
                _keyFrames.Add(frame);
                OnFreezablePropertyChanged(null, frame);
            }
        }

        #endregion

        #region ICollection (Explicit)

        void ICollection.CopyTo(Array array, int index)
        {
            ReadPreamble();
            _keyFrames.CopyTo((KeyFrame<T>[])array, index);
        }

        #endregion

        #region IList (Explicit)

        int IList.Add(object keyFrame)
        {
            Add((KeyFrame<T>)keyFrame);

            return _keyFrames.Count - 1;
        }

        bool IList.Contains(object keyFrame)
        {
            return Contains((KeyFrame<T>)keyFrame);
        }

        int IList.IndexOf(object keyFrame)
        {
            return IndexOf((KeyFrame<T>)keyFrame);
        }

        void IList.Insert(int index, object keyFrame)
        {
            Insert(index, (KeyFrame<T>)keyFrame);
        }

        void IList.Remove(object keyFrame)
        {
            Remove((KeyFrame<T>)keyFrame);
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (KeyFrame<T>)value; }
        }

        #endregion
    }
}