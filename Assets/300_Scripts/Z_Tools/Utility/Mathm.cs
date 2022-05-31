using UnityEngine;

namespace HorrorPS1.Tools
{
    /// <summary>
    /// Contains a bunch of useful math methods.
    /// </summary>
    public static class Mathm
    {
        #region Sign
        /// <summary>
        /// Get a boolean as a sign.
        /// 1 if true, -1 otherwise.
        /// </summary>
        /// <param name="_bool">Boolean to get sign from.</param>
        /// <returns>Returns this boolean sign as 1 or -1.</returns>
        public static int Sign(bool _bool)
        {
            return _bool ? 1 : -1;
        }

        /// <summary>
        /// Get the sign of an int.
        /// </summary>
        /// <param name="_value">Int to get sign.</param>
        /// <returns>-1 if smaller than 0, 1 otherwise.</returns>
        public static int Sign(int _value)
        {
            return _value < 0 ? -1 : 1;
        }

        /// <summary>
        /// Get the sign of a float.
        /// </summary>
        /// <param name="_value">Float to get sign.</param>
        /// <returns>-1 if smaller than 0, 1 otherwise.</returns>
        public static int Sign(float _value)
        {
            return _value < 0f ? -1 : 1;
        }

        /// <summary>
        /// Get if two floats have a different sign.
        /// </summary>
        /// <param name="_a">Float a to compare.</param>
        /// <param name="_b">Float b to compare.</param>
        /// <returns>Returns false if floats have the same sign, true otherwise./returns>
        public static bool HaveDifferentSign(float _a, float _b)
        {
            return Mathf.Sign(_a) != Mathf.Sign(_b);
        }

        /// <summary>
        /// Get if two floats have the same sign and are not null.
        /// </summary>
        /// <param name="_a">Float a to compare.</param>
        /// <param name="_b">Float b to compare.</param>
        /// <returns>Returns true if both floats do not equal 0 and have a different sign, false otherwise.</returns>
        public static bool HaveDifferentSignAndNotNull(float _a, float _b)
        {
            return ((_a != 0) && (_b != 0)) ? HaveDifferentSign(_a, _b) : false;
        }
        #endregion

        #region Null
        /// <summary>
        /// Get if three ints are all equal.
        /// </summary>
        /// <param name="_a">First int to compare.</param>
        /// <param name="_b">Second int to compare.</param>
        /// <param name="_c">Third int to compare.</param>
        /// <returns>True if all ints are equal, false otherwise.</returns>
        public static bool AreEquals(int _a, int _b, int _c)
        {
            bool _isNull = (_a == _b) && (_b == _c);
            return _isNull;
        }

        /// <summary>
        /// Get if three floats are all equal.
        /// </summary>
        /// <param name="_a">First float to compare.</param>
        /// <param name="_b">Second float to compare.</param>
        /// <param name="_c">Third float to compare.</param>
        /// <returns>True if all floats are equal, false otherwise.</returns>
        public static bool AreEqual(float _a, float _b, float _c)
        {
            bool _isNull = (_a == _b) && (_b == _c);
            return _isNull;
        }
        #endregion

        #region Vector3
        /// <summary>
        /// Get if a Vector3 is null, that is
        /// if its x, y & z values are equal to zero.
        /// </summary>
        public static bool IsVectorNull(Vector3 _vector)
        {
            return (_vector.x == 0) && (_vector.y == 0) && (_vector.z == 0);
        }

        /// <summary>
        /// Get if a Vector2 is null, that is
        /// if its x & y values are equal to zero.
        /// </summary>
        public static bool IsVectorNull(Vector2 _vector)
        {
            return (_vector.x == 0) && (_vector.y == 0);
        }

        /// <summary>
        /// Subtracts any part of the direction parallel to the normal,
        /// leaving only the perpendicular component.
        /// </summary>
        public static Vector3 ParallelSurface(Vector3 _direction, Vector3 _normal)
        {
            return _direction - (_normal * Vector3.Dot(_direction, _normal));
        }
        #endregion
    }
}
