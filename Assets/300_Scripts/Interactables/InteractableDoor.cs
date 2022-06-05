using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class InteractableDoor : MonoBehaviour
    {
        #region Fields and Properties
        [SerializeField] private SceneDataHandler sceneDataHandler = null;
        [SerializeField] private OpenDoors loadedSceneIndex = 0;
        [SerializeField] private Transform afterDoorTransform = null;
        #endregion

        #region Methods 

        #endregion
        public void Interact()
        {
            InGameState.Player.SetAfterLoadingTransform(afterDoorTransform);
            sceneDataHandler.ExitScene(loadedSceneIndex);
        }
    }
}
