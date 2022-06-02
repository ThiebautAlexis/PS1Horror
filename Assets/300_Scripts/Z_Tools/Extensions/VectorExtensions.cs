using System.Text;
using UnityEngine;

namespace HorrorPS1.Tools
{
    public static class VectorExtensions
    {
        #region Vector2
        /// <summary>
        /// Get a random value between a vector X & Y components.
        /// </summary>
        /// <param name="_vector">Vector to get random value from.</param>
        /// <returns>Random value between this vector X & Y components</returns>
        public static float Random(this Vector2 _vector)
        {
            return UnityEngine.Random.Range(_vector.x, _vector.y);
        }

        /// <summary>
        /// Return if the <see cref="_value"/> value is between the X & Y components of the Vector.
        /// </summary>
        /// <param name="_vector">Vector to get the bounds from.</param>
        /// <param name="_value">Tested value.</param>
        /// <param name="_useLimits">Are the X & Y values of the Vector are within the bounds.</param>
        /// <returns>Does the value is between the X & Y components of the Vector.</returns>
        public static bool ContainsValue(this Vector2 _vector, float _value, bool _useLimits = true)
        {
            if (_useLimits)
                return _value >= _vector.x && _value <= _vector.y;
            return _value > _vector.x && _value < _vector.y;
        }
        #endregion

        #region Vector3
        /// <summary>
        /// Get if a Vector3 is null, that is
        /// if its x, y & z values are equal to zero.
        /// </summary>
        public static bool IsNull(this Vector3 _vector) => Mathm.IsVectorNull(_vector);

        /// <summary>
        /// Subtracts any part of the direction parallel to the normal,
        /// leaving only the perpendicular component.
        /// </summary>
        public static Vector3 ParallelSurface(this Vector3 _vector, Vector3 _normal) => Mathm.ParallelSurface(_vector, _normal);

        /// <summary>
        /// Parse a Vector3 to a string with a defined amount of decimals.
        /// </summary>
        public static string ToStringX(this Vector3 _vector, int _decimals)
        {
            StringBuilder _string = new StringBuilder("0.");
            for (int _i = 0; _i < _decimals; _i++)
                _string.Append('#');

            return _vector.ToString(_string.ToString());
        }
        #endregion
    }
}
