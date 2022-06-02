using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    [CreateAssetMenu(fileName = "Player Rotation Data", menuName = "PS1 Horror Game/Data/Player/Rotation Data", order = 150)]
    public class PlayerRotationAttributes : ScriptableObject
    {
        #region Content 
        [Header("General Settings")]
        public float RotationSpeed = 15.0f;

        [Header("Cooldown Settings")]
        public float RotationCooldown = .01f;
        #endregion
    }
}
