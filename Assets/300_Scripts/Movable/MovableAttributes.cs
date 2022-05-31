using DG.Tweening;
using EnhancedEditor;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace HorrorPS1.Movable
{
    public class MovableAttributes : ScriptableObject
    {
        [Enhanced, MinMax(0f, 50f)] public Vector2 _speedRange = Vector2.zero;
        [Enhanced, Range(0f, 10f)] public float speedDuration = .7f;
        [Enhanced, EnhancedCurve(0f, 0f, 1f, 1f, SuperColor.Crimson)] public AnimationCurve speedCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        public float EvaluateSpeed(float time)
        {
            time = Mathf.Min(time, speedDuration);
            if (time != 0f)
            {
                time = time / speedDuration;
            }

            float value = DOVirtual.EasedValue(_speedRange.x, _speedRange.y, time, speedCurve);
            return value;
        }
    }
}
