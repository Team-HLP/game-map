using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities.UI
{
    /// <summary>
    /// Animates the offset of a Rect Transform.
    /// </summary>
    public class RectTransformOffsetAnimationController : AnimationController
    {

        [Tooltip("The Rect Transform whose offset is being animated.")]
        [SerializeField]
        protected RectTransform m_RectTransform;


        [Tooltip("The animation curve describing the offset value over the normalized animation time (0 - 1).")]
        [SerializeField]
        protected AnimationCurve offsetCurve = AnimationCurve.Linear(0, 1, 1, 0);


        // Called when this component is first added to a gameobject or reset in the inspector
        protected virtual void Reset()
        {
            m_RectTransform = GetComponentInChildren<RectTransform>();
        }


        protected override void OnAnimationPositionChanged()
        {
            SetOffset(offsetCurve.Evaluate(animationPosition));
        }


        // Set the Rect Transform's offset.
        protected virtual void SetOffset(float offset)
        {
            m_RectTransform.offsetMin = new Vector2(-offset, -offset);
            m_RectTransform.offsetMax = new Vector2(offset, offset);
        }
    }
}

