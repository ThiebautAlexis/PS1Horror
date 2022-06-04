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
        [SerializeField] private TMP_Text textDiplay = null;
        [SerializeField] private Image portaitDisplay = null;
        [SerializeField, Tooltip("Speed in letters per seconds"), Enhanced] private float displayingSpeed = 1f;
        [SerializeField] private Ease displayingEase = Ease.Linear;

        private Sequence textSequence = null;
        private DialogueData readData = null;
        private int readIndex = 0;
        #endregion

        #region Methods 
        
        [Button]
        public void ReadDialogue(DialogueData _dialogueAsset)
        {
            portaitDisplay.sprite = _dialogueAsset.Portrait;
            readData = _dialogueAsset;
            readIndex = 0;
            // Call Game State

            GoToNextLine();
        }


        public void SkipReading()
        {
            if (textSequence.IsActive())
            {
                textSequence.Kill(true);
                return;
            }

            GoToNextLine();
        }

        private void GoToNextLine()
        {
            if (readIndex >= readData.LinesID.Length)
            {
                EndReading();
                return;
            }

            string _line = readData.GetLine(readIndex);
            textDiplay.maxVisibleCharacters = 0;
            textDiplay.text = _line;

            textSequence = DOTween.Sequence();
            {
                textSequence.Join(DOTween.To(SetMaxVisibleCharacters, 0f, 1f, _line.Length * (1/displayingSpeed)).SetEase(displayingEase));
                textSequence.onComplete += OnSequenceCompleted;
            }

            void OnSequenceCompleted()
            {
                readIndex++;
            }

            void SetMaxVisibleCharacters(float _value)
            {
                textDiplay.maxVisibleCharacters = (int)(_value * textDiplay.text.Length) + 1;
            }
        }

        private void EndReading()
        {

        }
        #endregion
    }
}
