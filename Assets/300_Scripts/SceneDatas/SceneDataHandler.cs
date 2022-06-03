using EnhancedEditor;
using System;
using UnityEngine;
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
        public void OnEnterScene(int _cameraIndex = 0)
        {
            for (int i = 0; i < modifiers.Length; i++)
            {
                modifiers[i].ModifierTransform.gameObject.SetActive(sceneData.ModifiersValue > modifiers[i].ModifierThreshold);
            }
            GameState.ChangeCamera(sceneCameras[_cameraIndex]);
        }

        [Button("test")]
        public void OnExitScene(OpenDoors _door)
        {
            if ((sceneData.openedDoors & _door) > 0)
            {
                Debug.Log("oui");
            }
            //sceneData.openedDoors
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
