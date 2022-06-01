using EnhancedEditor;
using UnityEngine;

namespace HorrorPS1.Settings
{
    [CreateAssetMenu(fileName = "DAT_GameSettings", menuName = "Datas/Core/Game Settings", order = GameSettingsMenuOrder)]
    public class GameSettings : ScriptableObject
    {
        public const int GameSettingsMenuOrder = 150;

        [Section("Game Settings")]

        [Enhanced, Required] public PhysicsSettings PhysicsSettings = null;
    }
}
