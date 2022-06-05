using EnhancedEditor;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HorrorPS1
{
    public class LoadingSceneState : GameState
    {
        public override int Priority { get; protected set; } = 100;
        public override bool isActive { get; set; } = false;

        public static event Action OnStartSceneLoading;
        public static event Action OnEndSceneLoading;

        #region Fields and Properties
        private static SceneAsset loadedScene;
        private static Scene previousScene;
        #endregion

        #region Methods 
        public static void LoadScene(SceneAsset _loadedScene, Scene _previousScene)
        {
            GameStatesManager.SetStateActivation(GameStatesManager.LoadingSceneState, true);
            OnStartSceneLoading?.Invoke();

            loadedScene = _loadedScene;
            previousScene = _previousScene;

            LoadingSceneAsyncOperation _operation = new LoadingSceneAsyncOperation(loadedScene);
            _operation.OnSceneLoaded += UnloadPreviousScene;
        }

        private static void UnloadPreviousScene(SceneAsset obj)
        {
            SceneManager.UnloadSceneAsync(previousScene).completed += OnSceneLoaded;
        }

        public static void OnSceneLoaded(AsyncOperation _operation)
        {
            OnEndSceneLoading?.Invoke();
            GameStatesManager.SetStateActivation(GameStatesManager.LoadingSceneState, false);
        }


        #endregion
    }

    public class LoadingSceneAsyncOperation : AsyncOperation
    {
        private AsyncOperation currentOperation = null;
        private SceneAsset loadedScene;
        public event Action<SceneAsset> OnSceneLoaded = null;

        public LoadingSceneAsyncOperation(SceneAsset _loadedScene)
        {
            loadedScene = _loadedScene;
            LoadSceneParameters _params = new LoadSceneParameters()
            {
                loadSceneMode = LoadSceneMode.Additive
            };

            // Launch Scene Loading
            if (loadedScene.LoadAsync(_params, out currentOperation))
            {
                currentOperation.completed += OnOperationCompleted;
            }
        }


        private void OnOperationCompleted(AsyncOperation operation)
        {
            OnSceneLoaded?.Invoke(loadedScene);
        }
    }
}
