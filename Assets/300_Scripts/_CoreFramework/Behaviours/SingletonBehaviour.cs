using EnhancedEditor;
using UnityEngine;

namespace HorrorPS1.Core
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance = null;

        protected virtual void OnEnable()
        {
            Instance = this as T;
        }

        protected virtual void OnDisable()
        {
            Instance = null;
        }
    }
}
