using Cinemachine;
using EnhancedEditor;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HorrorPS1
{
    public static class GameState
    {
        #region Events 
        public static event Action<CinemachineVirtualCamera> OnCameraChange = null;
        #endregion

        #region Fields and Properties
        public static CinemachineVirtualCamera CurrentCamera = null;

        public static float Sanity { get; set; } = 0f;
        public static ConditionElements FullfilledConditions = 0;
        #endregion

        #region Methods 
        public static void ChangeCamera(CinemachineVirtualCamera _newCamera)
        {
            if(CurrentCamera != null) CurrentCamera.enabled = false;
            CurrentCamera = _newCamera;
            CurrentCamera.enabled = true;
            OnCameraChange?.Invoke(_newCamera); 
        }


        public static void FullfillCondition(ConditionElements _condition)
        {
            Debug.Log(FullfilledConditions);
            FullfilledConditions |= _condition;
            Debug.Log(FullfilledConditions);
        }
        public static void RemoveCondition(ConditionElements _condition) 
        {
            FullfilledConditions &= ~_condition;
        }

        #endregion
    }

    [Flags]
    public enum ConditionElements
    {
        Salt = 1,
        StaircaseKey = 2,
        BeenInBedroom = 4
    }
}
