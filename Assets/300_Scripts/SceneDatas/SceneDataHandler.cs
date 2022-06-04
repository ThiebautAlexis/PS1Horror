using EnhancedEditor;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

namespace HorrorPS1
{
    public class SceneDataHandler : MonoBehaviour
    {
        #region Fields and Properties
        [SerializeField] private SceneData sceneData = null;
        [SerializeField] private CinemachineVirtualCamera[] sceneCameras = new CinemachineVirtualCamera[] { };
        [SerializeField] private SceneModifier[] modifiers = new SceneModifier[] { };
        #endregion

        #region Methods 
        private void OnEnable()
        {
            OnEnterScene();
        }

        private void OnEnterScene(int _cameraIndex = 0)
        {
            for (int i = 0; i < modifiers.Length; i++)
            {
                modifiers[i].ModifierTransform.gameObject.SetActive(sceneData.ModifiersValue > modifiers[i].ModifierThreshold);
            }
            InGameState.ChangeCamera(sceneCameras[_cameraIndex]);
        }

        public void ExitScene(int _sceneIndex) => ExitScene((OpenDoors)_sceneIndex);

        public void ExitScene(OpenDoors _door)
        {
            if ((sceneData.openedDoors & _door) > 0 )
            {
                SceneAsset _loadedScene = null;
                switch (_door)
                {
                    case OpenDoors.One:
                        if (sceneData.LinkedScenes.Length > 0)
                            _loadedScene = sceneData.LinkedScenes[0];
                        break;
                    case OpenDoors.Two:
                        if (sceneData.LinkedScenes.Length > 1)
                            _loadedScene = sceneData.LinkedScenes[1];
                        break;
                    case OpenDoors.Three:
                        if (sceneData.LinkedScenes.Length > 2)
                            _loadedScene = sceneData.LinkedScenes[1];
                        break;
                    case OpenDoors.Four:
                        if (sceneData.LinkedScenes.Length > 3)
                            _loadedScene = sceneData.LinkedScenes[3];
                        break;
                    case OpenDoors.Five:
                        if (sceneData.LinkedScenes.Length > 4)
                            _loadedScene = sceneData.LinkedScenes[4];
                        break;
                    case OpenDoors.Six:
                        if (sceneData.LinkedScenes.Length > 5)
                            _loadedScene = sceneData.LinkedScenes[5];
                        break;
                    case OpenDoors.Seven:
                        if (sceneData.LinkedScenes.Length > 6)
                            _loadedScene = sceneData.LinkedScenes[6];
                        break;
                    case OpenDoors.Eight:
                        if (sceneData.LinkedScenes.Length > 7)
                            _loadedScene = sceneData.LinkedScenes[7];
                        break;
                    case OpenDoors.Nine:
                        if (sceneData.LinkedScenes.Length > 8)
                            _loadedScene = sceneData.LinkedScenes[8];
                        break;
                    default:
                        break;
                }
                if(_loadedScene != null)
                {
                    LoadingSceneState.LoadScene(_loadedScene, gameObject.scene);
                    return;
                }
            }
            // UI Feedback here
            InfoState.DisplayInteractionInfo("It's locked! But maybe I can find the key.");
        }

        public void SetCamera(int _cameraIndex)
        {
            InGameState.ChangeCamera(sceneCameras[_cameraIndex]);
        }
        #endregion
    }

    [Serializable]
    public class SceneModifier
    {
        public Transform ModifierTransform = null;
        public float ModifierThreshold = 0.0f;
    }

   
}
