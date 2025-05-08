using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities.UI
{
    /// <summary>
    /// Animate the alpha value of a canvas group.
    /// </summary>
    public class CanvasGroupFader : AnimationController
    {

        [Tooltip("The animation curve describing the alpha value over the normalized animation time (0 - 1).")]
        [SerializeField]
        protected AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);


        [Tooltip("The canvas group whose alpha value is being animated.")]
        [SerializeField]
        protected CanvasGroup canvasGroup;
        public CanvasGroup CanvasGroup
        {
            get { return canvasGroup; }
            set { canvasGroup = value; }
        }



        // Called when this component is first added to a gameobject or reset in the inspector
        protected virtual void Reset()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }



        /// <summary>
        /// Set the alpha value of the canvas group.
        /// </summary>
        /// <param name="alpha">The alpha value.</param>
        public virtual void SetAlpha(float alpha)
        {
            canvasGroup.alpha = alpha;
        }


        protected override void OnAnimationPositionChanged()
        {
            SetAlpha(alphaCurve.Evaluate(animationPosition));
        }
    }
}

