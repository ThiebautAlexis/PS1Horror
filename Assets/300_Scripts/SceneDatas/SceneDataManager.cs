using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class SceneDataManager : MonoBehaviour
    {
        #region Fields and Properties
        [SerializeField] private SceneBundle linkedScenes = null;
        [SerializeField] private Camera[] sceneCameras = new Camera[] { };
        #endregion

        #region Methods 
        public void OnEnterScene()
        {
            GameState.ChangeCamera(sceneCameras[0]);
            linkedScenes.LoadAsync();
        }

        public void OnExitScene(int _nextSceneIndex = 0)
        {
            linkedScenes.UnloadAsync(_nextSceneIndex);
        }
        #endregion
    }
}
