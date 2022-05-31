using EnhancedEditor;
using UnityEngine;

using Min = EnhancedEditor.MinAttribute;
using Range = EnhancedEditor.RangeAttribute;

namespace HorrorPS1.Settings
{
    [CreateAssetMenu(fileName = "DAT_PhysicsSettings", menuName = "Datas/Core/Physics Settings", order = GameSettings.GameSettingsMenuOrder)]
	public class PhysicsSettings : ScriptableObject
    {
        /// <summary>
        /// Game used global Physics settings.
        /// </summary>
        public static PhysicsSettings I;

        #region Settings
        [Section("Physics Settings")]

        [Enhanced, Max(0f)] public float MaxGravity = -25f;

        [Space]

        [Enhanced, Range(.1f, 1f)] public float GroundMinNormal = .85f;
        [Enhanced, Min(0f)] public float GroundClimbHeight = .2f;
        [Enhanced, Min(0f)] public float GroundSnapHeight = .2f;

        [Space]

        [Enhanced, Min(0f)] public float SteepSlopeRequiredMovement = 20f;
        [Enhanced, Min(0f)] public float SteepSlopeRequiredForce = 10f;

        [HorizontalLine(SuperColor.Sapphire)]

        [Enhanced, Range(0f, 1f)] public float OnGroundedForceMultiplier = .55f;

        [Space]

        [Enhanced, Min(0f)] public float GroundDecelerationForce = 17f;
        [Enhanced, Min(0f)] public float AirDecelerationForce = 5f;
        #endregion
    }
}
