using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VSX.RadarSystem
{
    /// <summary>
    /// Manages a single lead target box on a target box on the HUD.
    /// </summary>
    public class HUDTargetBox_LeadTargetBoxController : MonoBehaviour
    {

        [SerializeField]
        protected Image box;
        public Image Box { get { return box; } }

        [SerializeField]
        protected Vector2 unaimedBoxSize = new Vector2(20, 20);

        [SerializeField]
        protected Vector2 aimedBoxSize = new Vector2(30, 30);

        [SerializeField]
        protected float boxSizeAnimationDuration = 0.5f;

        [SerializeField]
        protected Image line;
        public Image Line { get { return line; } }

        [Tooltip("This is the value that is multiplied by the ratio of the line width to the box width, to determine the alpha. Fades out the lead target box near the center.")]
        [SerializeField]
        protected float lineLengthToBoxWidthAlphaCoefficient = 1f;

        [SerializeField]
        protected CanvasGroup boxFadeCanvasGroup;

        [Tooltip("The maximum alpha for the lead target box.")]
        [SerializeField]
        protected float maxAlpha = 1f;
        

        /// <summary>
        /// Update the lead target box
        /// </summary>
        public virtual void UpdateLeadTargetBox(bool isAimed, float aimStateStartTime)
        {
            // Set the line position
            line.rectTransform.localPosition = 0.5f * box.rectTransform.localPosition;

            // Set the rotation
            if ((box.rectTransform.position - box.rectTransform.parent.position).magnitude < 0.0001f)
            {
                line.rectTransform.rotation = Quaternion.identity;
            }
            else
            {
                line.rectTransform.rotation = Quaternion.LookRotation(box.rectTransform.position - box.rectTransform.parent.position, Vector3.up);
            }
            line.transform.Rotate(Vector3.up, 90, UnityEngine.Space.Self);

            // Set the line size
            Vector2 size = line.rectTransform.sizeDelta;
            size.x = 2 * Vector3.Magnitude(line.rectTransform.localPosition);

            line.rectTransform.sizeDelta = size;

            // Fade the lead target box/line when near the center
            if (box.rectTransform.sizeDelta.x > 0.0001f)
            {
                Color c = line.color;
                c.a = Mathf.Clamp(lineLengthToBoxWidthAlphaCoefficient * (line.rectTransform.sizeDelta.x / box.rectTransform.sizeDelta.x), 0, maxAlpha);
                line.color = c;

                float alpha = Mathf.Clamp(lineLengthToBoxWidthAlphaCoefficient * (line.rectTransform.sizeDelta.x / box.rectTransform.sizeDelta.x), 0, maxAlpha);
                if (boxFadeCanvasGroup != null)
                {
                    boxFadeCanvasGroup.alpha = alpha;
                }
                else
                {
                    c = box.color;
                    c.a = alpha;
                    box.color = c;
                }
            }


            // Animate the lead target box size

            float amount = Mathf.Clamp((Time.time - aimStateStartTime) / boxSizeAnimationDuration, 0, 1);

            Vector2 boxSize = isAimed ? (amount * aimedBoxSize + (1 - amount) * unaimedBoxSize) : (amount * unaimedBoxSize + (1 - amount) * aimedBoxSize);

            box.rectTransform.sizeDelta = boxSize;
        }

        /// <summary>
        /// Activate the lead target box.
        /// </summary>
        public void Activate()
        {
            box.gameObject.SetActive(true);
            line.gameObject.SetActive(true);
        }

        /// <summary>
        /// Deactivate the lead target box.
        /// </summary>
        public void Deactivate()
        {
            box.gameObject.SetActive(false);
            line.gameObject.SetActive(false);
        }
    }
}