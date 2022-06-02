using EnhancedEditor;
using HorrorPS1.Settings;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace HorrorPS1.Core
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        #region Global Members
        [Section("Game Manager")]

        [SerializeField, Enhanced, ValidationMember("UseVSync")] private bool useVSync = false;
        [SerializeField, Enhanced, ValidationMember("TargetFramerate"), Range(0f, 200f)] private int targetFramerate = 60;
        [SerializeField, Enhanced, ValidationMember("Build Scene Database Asset")] private BuildSceneDatabase buildSceneDatabase = null;
        public bool UseVSync
        {
            get => useVSync;
            set
            {
                useVSync = value;
                QualitySettings.vSyncCount = value ? 0 : 1;
            }
        }

        public int TargetFramerate
        {
            get => targetFramerate;
            set
            {
                targetFramerate = value;
                Application.targetFrameRate = value;
            }
        }

        // -----------------------

        [HorizontalLine(SuperColor.Green)]

        [SerializeField, Enhanced, Required] private GameSettings gameSettings = null;

        public GameSettings GameSettings => gameSettings;

        // -----------------------

        /// <summary>
        /// Is the application currently being shut down?.
        /// </summary>
        public static bool IsQuittingApplication { get; private set; } = false;
        #endregion

        #region Mono Behaviour
        protected virtual void Awake()
        {
            PhysicsSettings.I = gameSettings.PhysicsSettings;
            BuildSceneDatabase.Database = buildSceneDatabase;
        }

        protected virtual void OnApplicationQuit()
        {
            IsQuittingApplication = true;
        }
        #endregion
    }
}
