using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    public class SceneDoor : MonoBehaviour, IInteractable
    {
        #region Fields and Properties
        [SerializeField] private SceneDataHandler handler = null;
        [SerializeField] private OpenDoors doorIndex = OpenDoors.One;
        // [SerializeField] private bool useFadeToBlack = false;
        #endregion

        #region Methods 

        #endregion
        public void CancelInteraction()
        {
        }

        public void Interact()
        {
            handler.ExitScene(doorIndex);
        }

        private void OnDrawGizmos()
        {
            if (!handler) return;
            switch (doorIndex)
            {
                case OpenDoors.One:
                    Gizmos.color = Color.red;
                    break;
                case OpenDoors.Two:
                    Gizmos.color = Color.blue;
                    break;
                case OpenDoors.Three:
                    Gizmos.color = Color.green;
                    break;
                case OpenDoors.Four:
                    Gizmos.color = Color.cyan;
                    break;
                case OpenDoors.Five:
                    Gizmos.color = Color.magenta;
                    break;
                case OpenDoors.Six:
                    Gizmos.color = Color.yellow;
                    break;
                case OpenDoors.Seven:
                    Gizmos.color = Color.white;                    
                    break;
                case OpenDoors.Eight:
                    Gizmos.color = Color.black;
                    break;
                case OpenDoors.Nine:
                    Gizmos.color = Color.red;
                    break;
                default:
                    return;
            }
            Gizmos.DrawLine(transform.position, handler.transform.position);
        }
    }
}
