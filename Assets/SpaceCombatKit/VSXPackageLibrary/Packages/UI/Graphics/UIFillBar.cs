using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UI
{
    public class UIFillBar : MonoBehaviour
    {

        [Tooltip("Whether the fill bar should fill in steps.")]
        [SerializeField]
        protected bool stepped = false;

        [Tooltip("The number of steps in a full bar.")]
        [SerializeField]
        protected int steps = 10;


        public virtual void SetFill(float fillAmount) { }


        protected virtual float ApplyStepping(float fill)
        {
            return ((float)(Mathf.RoundToInt(fill * steps))) / steps;
        }


        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}