using System;

namespace HorrorPS1.Tools
{
    /// <summary>
    /// Array wrapper which is automatically expanded by a fixed amount
    /// each time it is needed, without shrinking back when removing element.
    /// 
    /// Can be cleared to remove empty spaces.
    /// </summary>
    /// <typeparam name="T">Array element type.</typeparam>
    [Serializable]
	public class Buffer<T>
    {
        #region Buffer
        public const int DefaultExpandSize = 3;

        // -----------------------

        private readonly int expandSize = DefaultExpandSize;

        /// <summary>
        /// Total count of the buffer.
        /// </summary>
        public int Count = 0;

        /// <summary>
        /// Array of the buffer. Its size should not be set externally.
        /// </summary>
        public T[] Array = new T[] { };
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new buffer (one way expanding array).
        /// </summary>
        /// <param name="_expandSize">Amount by wich the array is expanded
        /// each time it needs more space.</param>
        public Buffer(int _expandSize = DefaultExpandSize)
        {
            expandSize = _expandSize;
        }

        /// <summary>
        /// Creates a new buffer (one way expanding array).
        /// </summary>
        /// <param name="_size">Initial size of the buffer.</param>
        /// <param name="_expandSize">Amount by wich the array is expanded
        /// each time it needs more space.</param>
        public Buffer(int _size, int _expandSize) : this(_expandSize)
        {
            Array = new T[_size];
            Count = _size;
        }
        #endregion

        #region Operators
        public T this[int _index]
        {
            get => Array[_index];
            set => Array[_index] = value;
        }
        #endregion

        #region Management
        /// <summary>
        /// Adds a new element in the array.
        /// </summary>
        /// <param name="_element">New array element.</param>
        public void Add(T _element)
        {
            if (Array.Length == Count)
                Expand();

            Array[Count] = _element;
            Count++;
        }

        /// <summary>
        /// Tries to restore last removed element from the buffer.
        /// Automatically add it to the buffer is succesfully restored.
        /// </summary>
        /// <param name="_element">Restored element.</param>
        /// <returns>True if successfully restored element, false otherwise.</returns>
        public bool Restore(out T _element)
        {
            if ((Array.Length == Count) || ReferenceEquals(Array[Count], null))
            {
                _element = default;
                return false;
            }

            _element = Array[Count];
            Count++;

            return true;
        }

        /// <summary>
        /// Removes an element from the array.
        /// </summary>
        /// <param name="_element">Element to remove.</param>
        public void Remove(T _element)
        {
            int _index = System.Array.IndexOf(Array, _element);
            RemoveAt(_index);
        }

        /// <summary>
        /// Removes the element at specified index from the array.
        /// </summary>
        /// <param name="_element">Index of the element to remove.</param>
        public void RemoveAt(int _index)
        {
            int _last = Count - 1;
            if (_index != _last)
            {
                T _removed = Array[_index];

                Array[_index] = Array[_last];
                Array[_last] = _removed;
            }

            Count--;
        }

        // -----------------------

        /// <summary>
        /// Clear this collection by setting its count to zero.
        /// </summary>
        public void Clear()
        {
            Count = 0;
        }

        /// <summary>
        /// Completely resets this buffer by removing all entries and setting its count to 0.
        /// </summary>
        public void Reset()
        {
            Array = new T[] { };
            Count = 0;
        }

        /// <summary>
        /// Resize this collection content by removing empty entries.
        /// Does not delete existing elements.
        /// </summary>
        public void Resize()
        {
            System.Array.Resize(ref Array, Count);
        }

        // -----------------------

        private void Expand()
        {
            System.Array.Resize(ref Array, Count + expandSize);
        }
        #endregion

        #region Utility
        /// <summary>
        /// Get if a given element is contained within the buffer.
        /// </summary>
        /// <param name="_element">Element to check if the buffer contain it.</param>
        /// <returns>True if the buffer contains the element, false otherwise.</returns>
        public bool Contains(T _element)
        {
            for (int _i = 0; _i < Count; _i++)
            {
                if (Array[_i].Equals(_element))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Modifies this buffer to match a given template content and size.
        /// </summary>
        /// <param name="_template">Template to copy.</param>
        public void Copy(Buffer<T> _template)
        {
            // Resize array and copy each element.
            Count = _template.Count;
            if (Array.Length < Count)
                Resize();

            for (int _i = 0; _i < Count; _i++)
            {
                Array[_i] = _template[_i];
            }
        }
        #endregion
    }
}
