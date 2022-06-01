using System;
using System.Collections.Generic;

namespace HorrorPS1.Tools
{
	public static class CollectionExtensions
    {
        #region Random
        /// <summary>
        /// Get a random element from a given array.
        /// </summary>
        /// <typeparam name="T">Content type of the array.</typeparam>
        /// <param name="_array">Array to get random element from.</param>
        /// <returns>Random element from given array.</returns>
        public static T Random<T>(this T[] _array)
        {
            return _array[UnityEngine.Random.Range(0, _array.Length)];
        }

        /// <summary>
        /// Get a random element from a given list.
        /// </summary>
        /// <typeparam name="T">Content type of the list.</typeparam>
        /// <param name="_list">List to get random element from.</param>
        /// <returns>Random element from given list.</returns>
        public static T Random<T>(this List<T> _list)
        {
            return _list[UnityEngine.Random.Range(0, _list.Count)];
        }
        #endregion

        #region Enumeration
        /// <summary>
        /// Get last element from an array.
        /// </summary>
        /// <typeparam name="T">Content type of the array.</typeparam>
        /// <param name="_array">Array to get last element from.</param>
        /// <returns>Last element from the given array.</returns>
        public static T Last<T>(this T[] _array)
        {
            return _array[_array.Length - 1];
        }

        /// <summary>
        /// A safe version of <see cref="Last{T}(T[])"/>.
        /// Get last element from an array or default value if empty.
        /// </summary>
        /// <typeparam name="T">Content type of the array.</typeparam>
        /// <param name="_array">Array to get last element from.</param>
        /// <returns>Last element from the given array or default value if empty.</returns>
        public static T SafeLast<T>(this T[] _array)
        {
            if (_array.Length == 0)
                return default(T);

            return _array[_array.Length - 1];
        }

        /// <summary>
        /// Get last element from a list.
        /// </summary>
        /// <typeparam name="T">Content type of the list.</typeparam>
        /// <param name="_array">List to get last element from.</param>
        /// <returns>Last element from the given list.</returns>
        public static T Last<T>(this List<T> _list)
        {
            return _list[_list.Count - 1];
        }

        /// <summary>
        /// A safe version of <see cref="Last{T}(List{T})"/>
        /// Get last element from a list or default value if empty.
        /// </summary>
        /// <typeparam name="T">Content type of the list.</typeparam>
        /// <param name="_array">List to get last element from.</param>
        /// <returns>Last element from the given list or default value if empty.</returns>
        public static T SafeLast<T>(this List<T> _list)
        {
            if (_list.Count == 0)
                return default(T);

            return _list[_list.Count - 1];
        }
        #endregion

        #region Utility
        /// <summary>
        /// Finds a specific element within this array.
        /// </summary>
        /// <param name="_match">Predicate to find matching element.</param>
        /// <param name="_element">Matching element.</param>
        /// <returns>True if found a matching element, false otherwise.</returns>
        public static bool Find<T>(this T[] _array, Predicate<T> _match, out T _element)
        {
            for (int _i = 0; _i < _array.Length; _i++)
            {
                _element = _array[_i];
                if (_match(_element))
                    return true;
            }

            _element = default;
            return false;
        }
        #endregion
    }
}
