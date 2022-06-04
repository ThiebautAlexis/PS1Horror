using DG.Tweening;
using EnhancedEditor;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HorrorPS1
{
    public class WalkieTalkie : MonoBehaviour
    {
        #region Fields and Properties
        #endregion

        #region Methods 
        
        [Button]
        public void StartReading(DialogueData _dialogueAsset)
        {
            // Call Game State
            WalkieTalkieState.ReadDialogueLine(_dialogueAsset);
        }


        public void SkipReading()
        {
            WalkieTalkieState.SkipDialogueLine();
        }
        #endregion
    }
}
