using HorrorPS1.Settings;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HorrorPS1.HorrorPhysics
{
    #region Raycast Hit Comparer
    /// <summary>
    /// Comparer for <see cref="RaycastHit"/> by distance.
    /// </summary>
    internal class RaycastHitDistanceComparer : IComparer<RaycastHit>
    {
        public static readonly RaycastHitDistanceComparer Default = new RaycastHitDistanceComparer();

        public int Compare(RaycastHit _a, RaycastHit _b)
        {
            return _a.distance.CompareTo(_b.distance);
        }
    }
    #endregion

    /// <summary>
    /// Contains a bunch of usefull methods related to the game Physics.
    /// </summary>
    public static class PhysicsUtility
    {
        #region Raycast Hits
        /// <summary>
        /// Sorst an array of RaycastHit by their distance.
        /// </summary>
        public static void SortRaycastHitByDistance(RaycastHit[] _hits, int _amount)
        {
            Array.Sort(_hits, 0, _amount, RaycastHitDistanceComparer.Default);
        }
        #endregion

        #region Collision Masks
        /// <summary>
        /// Get the collision layer mask that indicates which layer(s) the specified <see cref="GameObject"/> can collide with.
        /// </summary>
        /// <param name="_layer">The layer to retrieve the collision layer mask for.</param>
        public static int GetLayerCollisionMask(GameObject _gameObject)
        {
            int _layer = _gameObject.layer;
            return GetLayerCollisionMask(_layer);
        }

        /// <summary>
        /// Get the collision layer mask that indicates which layer(s) the specified layer can collide with.
        /// </summary>
        /// <param name="_layer">The layer to retrieve the collision layer mask for.</param>
        public static int GetLayerCollisionMask(int _layer)
        {
            int _layerMask = 0;
            for (int _i = 0; _i < 32; _i++)
            {
                if (!Physics.GetIgnoreLayerCollision(_layer, _i))
                    _layerMask |= 1 << _i;
            }

            return _layerMask;
        }
        #endregion

        #region Utility
        /// <summary>
        /// Get if a normal surface should be considered as ground or not.
        /// </summary>
        public static bool IsGroundSurface(Vector3 _normal)
        {
            return !(_normal.y < PhysicsSettings.I.GroundMinNormal);
        }
        #endregion
    }
}
