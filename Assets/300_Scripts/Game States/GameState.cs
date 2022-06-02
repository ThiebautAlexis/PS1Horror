using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public static class GameState
    {
        #region Events 
        public static event Action<Camera> OnCameraChange = null;
        #endregion

        #region Fields and Properties
        public static Camera CurrentCamera = null;
        #endregion

        #region Methods 
        public static void ChangeCamera(Camera _newCamera)
        {
            CurrentCamera = _newCamera;
            OnCameraChange?.Invoke(_newCamera); 
        }
        #endregion
    }
}
