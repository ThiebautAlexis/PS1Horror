using UnityEngine;

namespace HorrorPS1.Tools
{
	public static class AnimationCurveExtensions
    {
        /// <summary>
        /// Get the total duration of a given <see cref="AnimationCurve"/>.
        /// </summary>
        /// <param name="_curve">Curve to get duration.</param>
        /// <returns>Given curve duration.</returns>
        public static float Duration(this AnimationCurve _curve)
        {
            return _curve[_curve.length - 1].time;
        }

        /// <summary>
        /// Get the first key value of this curve.
        /// </summary>
        /// <param name="_curve">Curve to get first value.</param>
        /// <returns>First key value of this curve.</returns>
        public static float First(this AnimationCurve _curve)
        {
            return _curve[0].value;
        }

        /// <summary>
        /// Get the last key value of this curve.
        /// </summary>
        /// <param name="_curve">Curve to get last value.</param>
        /// <returns>Last key value of this curve.</returns>
        public static float Last(this AnimationCurve _curve)
        {
            return _curve[_curve.length - 1].value;
        }
    }
}
