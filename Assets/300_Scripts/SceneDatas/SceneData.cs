using Cinemachine;
using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class SceneData : MonoBehaviour
    {
        #region Fields and Properties
        [SerializeField] private SceneBundle linkedScenes = null;
        [SerializeField] private CinemachineVirtualCamera[] sceneCameras = new CinemachineVirtualCamera[] { };
        [SerializeField] private SceneModifier[] modifiers = new SceneModifier[] { };
        [SerializeField, Enhanced, ReadOnly] private float modifiersValue = 0f;
        #endregion

        #region Methods 
        public void OnEnterScene(int _cameraIndex = 0)
        {
            for (int i = 0; i < modifiers.Length; i++)
            {
                modifiers[i].ModifierTransform.gameObject.SetActive(modifiersValue > modifiers[i].ModifierThreshold);
            }
            GameState.ChangeCamera(sceneCameras[_cameraIndex]);
            linkedScenes.LoadAsync();
        }

        public void OnExitScene(int _nextSceneIndex = 0)
        {
            linkedScenes.UnloadAsync(_nextSceneIndex);
        }

        [Button("Aquamarine")]
        public void SetCamera(int _cameraIndex)
        {
            GameState.ChangeCamera(sceneCameras[_cameraIndex]); 
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
