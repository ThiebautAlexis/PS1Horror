using EnhancedEditor;
using System;
using UnityEngine;

namespace HorrorPS1
{
    [CreateAssetMenu(fileName = "SceneData_", menuName = "PS1 Horror Game/Scene Data", order = 150)]
    public class SceneData : ScriptableObject
    {
        #region Content
        public SceneAsset[] LinkedScenes = new SceneAsset[] { };
        [Enhanced, ReadOnly] public float ModifiersValue = 0f;
        public OpenDoors openedDoors;
        #endregion

        [Button("Unlock Door at index")]
        public void UnlockDoor(int _unlockedIndex)
        {
            switch (_unlockedIndex)
            {
                case 1:
                    openedDoors = openedDoors | OpenDoors.One;
                    break;
                case 2:
                    openedDoors = openedDoors | OpenDoors.Two;
                    break;
                case 3:
                    openedDoors = openedDoors | OpenDoors.Three;
                    break;
                case 4:
                    openedDoors = openedDoors | OpenDoors.Four;
                    break;
                case 5:
                    openedDoors = openedDoors | OpenDoors.Five;
                    break;
                case 6:
                    openedDoors = openedDoors | OpenDoors.Six;
                    break;
                case 7:
                    openedDoors = openedDoors | OpenDoors.Seven;
                    break;
                case 8:
                    openedDoors = openedDoors | OpenDoors.Eight;
                    break;
                case 9:
                    openedDoors = openedDoors | OpenDoors.Nine;
                    break;
                default:
                    break;
            }
        }

        [Button("Lock Door")]
        public void LockDoor(int _lockedIndex)
        {
            switch (_lockedIndex)
            {
                case 1:
                    openedDoors &= ~OpenDoors.One;
                    break;
                case 2:
                    openedDoors &= ~OpenDoors.Two;
                    break;
                case 3:
                    openedDoors &= ~OpenDoors.Three;
                    break;
                case 4:
                    openedDoors &= ~OpenDoors.Four;
                    break;
                case 5:
                    openedDoors &= ~OpenDoors.Five;
                    break;
                case 6:
                    openedDoors &= ~OpenDoors.Six;
                    break;
                case 7:
                    openedDoors &= ~OpenDoors.Seven;
                    break;
                case 8:
                    openedDoors &= ~OpenDoors.Eight;
                    break;
                case 9:
                    openedDoors &= ~OpenDoors.Nine;
                    break;
                default:
                    break;
            }
        }

    }

    [Flags]
    public enum OpenDoors
    {
        One = 1,
        Two = 2,
        Three = 4,
        Four = 8,
        Five = 16,
        Six = 32,
        Seven = 64,
        Eight = 128,
        Nine = 256
    }
}
