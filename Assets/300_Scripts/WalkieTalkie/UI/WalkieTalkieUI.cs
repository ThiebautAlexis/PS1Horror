using DG.Tweening;
using EnhancedEditor;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HorrorPS1
{
    public class WalkieTalkieUI : MonoBehaviour
    {
        #region Fields and Properties
        [SerializeField] private CanvasGroup group = null;
        [SerializeField] private TMP_Text textDiplay = null;
        [SerializeField] private Image portaitDisplay = null;
        [SerializeField, Tooltip("Speed in letters per seconds"), Enhanced] private float displayingSpeed = 1f;
        [SerializeField] private Ease displayingEase = Ease.Linear;

        private Sequence textSequence = null;
        private DialogueData readData = null;
        private int readIndex = 0;
        #endregion

        #region Methods 
        private void OnEnable()
        {
            WalkieTalkieState.OnReadDialogueLine += StartDialogue;
            WalkieTalkieState.OnSkipDialogueLine += Skip;
            GameStatesManager.OnChangeState += CheckStateChange;
        }
        
        private void OnDisable()
        {
            WalkieTalkieState.OnReadDialogueLine -= StartDialogue;
            WalkieTalkieState.OnSkipDialogueLine -= Skip;
            GameStatesManager.OnChangeState -= CheckStateChange;
        }

        private void StartDialogue(DialogueData _dialogueAsset)
        {
            portaitDisplay.sprite = _dialogueAsset.Portrait;
            readData = _dialogueAsset;
            readIndex = 0;
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
                textSequence.Join(DOTween.To(SetMaxVisibleCharacters, 0f, 1f, _line.Length * (1 / displayingSpeed)).SetEase(displayingEase));
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

        private void Skip()
        {
            if (textSequence.IsActive())
            {
                textSequence.Kill(true);
                return;
            }
            GoToNextLine();
        }

        private void EndReading()
        {
            // Call Game State
            WalkieTalkieState.EndDialogue();
        }

        private void CheckStateChange(Type _t)
        {
            if(_t == GameStatesManager.PauseState)
            {
                if (textSequence.IsActive())
                    textSequence.Pause();
            }
            if(_t == GameStatesManager.WalkieTalkieState)
            {
                if (textSequence.IsActive())
                    textSequence.Play();
                else
                {
                    group.alpha = 1f;
                }
            }
            else
            {
                group.alpha = 0f;
            }
        }
        #endregion
    }
}
