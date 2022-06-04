using EnhancedEditor;
using System;
using UnityEngine;
using TMPro;

namespace HorrorPS1
{
    public class InfoBoxUI : MonoBehaviour
    {
        #region Fields and Properties
        [SerializeField] private CanvasGroup group = null;
        [SerializeField] private TMP_Text text = null;
        #endregion

        #region Methods 

        private void OnEnable()
        {
            InfoState.OnDisplayInfo += DisplayInfo;
            InfoState.OnCloseInfo += CloseInfo;
            GameStatesManager.OnChangeState += OnChangeState;
        }

        private void OnDisable()
        {
            InfoState.OnDisplayInfo -= DisplayInfo;
            InfoState.OnCloseInfo -= CloseInfo;
            GameStatesManager.OnChangeState -= OnChangeState;
        }

        private void DisplayInfo(string _info)
        {
            text.text = _info;
        }

        private void CloseInfo() => group.alpha = 0f;

        private void OnChangeState(Type _t)
        {
            if (_t == GameStatesManager.InfoState)
                group.alpha = 1f;
            else
                group.alpha = 0f;
        }
        #endregion
    }
}
