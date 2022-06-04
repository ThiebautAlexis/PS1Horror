using EnhancedEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HorrorPS1
{
    public static class GameStatesManager
    {
        public static event Action<Type> OnChangeState;


        public static readonly Type InGameState = typeof(InGameState);              // 0
        public static readonly Type WalkieTalkieState = typeof(WalkieTalkieState);  // 1
        public static readonly Type InfoState = typeof(InfoState);                  // 2
        public static readonly Type PauseState = typeof(PauseState);                // 99
        public static readonly Type LoadingSceneState = typeof(LoadingSceneState);  // 100



        private static GameState[] gameStates = new GameState[]{new InGameState(), 
                                                                new WalkieTalkieState(), 
                                                                new PauseState(),
                                                                new InfoState(),
                                                                new LoadingSceneState()};

        public static Type currentGameState;

        public static void SetStateActivation(Type _t, bool _isActive)
        {
            for (int i = 0; i < gameStates.Length; i++)
            {
                if (gameStates[i].GetType() == _t)
                {
                    gameStates[i].isActive = _isActive;
                }
            }
            SelectNewState();
        }
        
        private static void SelectNewState()
        {
            int _bestPriority = -99;
            for (int i = 0; i < gameStates.Length; i++)
            {
                if (gameStates[i].isActive && gameStates[i].Priority > _bestPriority)
                {
                    currentGameState = gameStates[i].GetType();
                    _bestPriority = gameStates[i].Priority;
                }
            }
            OnChangeState?.Invoke(currentGameState);
        }
    }
}
