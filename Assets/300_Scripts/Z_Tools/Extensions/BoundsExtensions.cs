using UnityEngine;

namespace HorrorPS1.Tools
{
	public static class BoundsExtensions
    {
        /// <summary>
        /// The world space center position of the bounding box.
        /// </summary>
        /// <param name="_bounds">Bounds to get position.</param>
        /// <param name="_transform">Transform this bounds is attached to.</param>
        /// <returns>World space position of these bounds.</returns>
        public static Vector3 Position(this Bounds _bounds, Transform _transform)
        {
            return _bounds.center + _transform.position;
        }

        /// <summary>
        /// Get the world space max position of this bounding box.
        /// </summary>
        /// <param name="_bounds">Bounds to get max position.</param>
        /// <param name="_transform">Transform this bounds is attached to.</param>
        /// <param name="_sign">1 for right-side oriented position, -1 for left.</param>
        /// <returns>World space max position of these bounds.</returns>
        public static Vector3 Max(this Bounds _bounds, Transform _transform, float _sign = 1f)
        {
            Vector3 _pos = _bounds.Position(_transform);
            _pos += _bounds.extents * ((_sign < 0f) ? -1f : 1f);

            return _pos;
        }

        /// <summary>
        /// Get these bounds in world space coordinates.
        /// </summary>
        /// <param name="_bounds">Bounds to get coordinates.</param>
        /// <param name="_transform">Transform this bounds is attached to.</param>
        /// <returns>World space coordinates of these bounds.</returns>
        public static Bounds ToWorld(this Bounds _bounds, Transform _transform)
        {
            Bounds _worldSpace = new Bounds(_bounds.center + _transform.position, _bounds.size);
            return _worldSpace;
        }

        /// <summary>
        /// Does another bounding box intersects with this bounding box?
        /// </summary>
        /// <param name="_bounds">Bounds to use as reference.</param>
        /// <param name="_transform">Transform this reference bounds is attached to.</param>
        /// <param name="_other">Other bounds to test intersection.</param>
        /// <param name="_otherTransform">Transform the other bounds is attached to.</param>
        /// <returns>True if the two bounds do intersect, false otherwise.</returns>
        public static bool Intersects(this Bounds _bounds, Transform _transform, Bounds _other, Transform _otherTransform)
        {
            Bounds _a = _bounds.ToWorld(_transform);
            Bounds _b = _other.ToWorld(_otherTransform);

            return _a.Intersects(_b);
        }
    }
}
