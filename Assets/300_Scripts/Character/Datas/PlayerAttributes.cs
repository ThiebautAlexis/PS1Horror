using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    [CreateAssetMenu(fileName = "Player Data", menuName = "PS1 Horror Game/Data/Player Data", order = 150)]
    public class PlayerAttributes : ScriptableObject
    {
        #region Content 
        [Header("Sprint")]
        public float SprintMultiplier = 1.2f;
        public float SprintLimit = 5.0f;

        [Header("Rotation Settings")]
        public float RotationSpeed = 15.0f;
        public float RotationThreshold = 5.0f;
        public float RotationCooldown = .01f;
        #endregion
    }
}
