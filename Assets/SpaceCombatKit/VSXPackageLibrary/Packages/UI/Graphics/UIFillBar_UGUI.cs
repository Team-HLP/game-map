using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VSX.UI
{
    public class UIFillBar_UGUI : UIFillBar
    {

        [SerializeField]
        protected Image barFill;


        protected virtual void Reset()
        {
            barFill = GetComponentInChildren<Image>();
        }

        public override void SetFill(float fillAmount)
        {
            barFill.fillAmount = stepped ? ApplyStepping(fillAmount) : fillAmount;
        }
    }
}