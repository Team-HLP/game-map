using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.UI;

namespace VSX.Engines3D
{
    /// <summary>
    /// Display speed information for a Vehicle Engines 3D component on the HUD.
    /// </summary>
    public class HUDSpeedDisplayController : MonoBehaviour 
    {
        [Tooltip("The engines component to display speed information for.")]
        [SerializeField]
        protected VehicleEngines3D engines;


        [Tooltip("Speed fill bar.")]
        [SerializeField]
        protected UIFillBar speedBar;


        [Tooltip("Speed display text.")]
        [SerializeField]
        protected UVCText speedText;


        // Update is called once per frame
        protected virtual void Update() 
        {
            UpdateDisplay();    
        }


        protected virtual void UpdateDisplay()
        {
            if (engines != null)
            {
                if (speedBar != null)
                {
                    speedBar.SetFill(engines.Rigidbody.velocity.magnitude / engines.GetCurrentMaxSpeedByAxis(false).z);
                }

                if (speedText != null)
                {
                    speedText.text = ((int)engines.Rigidbody.velocity.magnitude).ToString();
                }
            }
        }
    }
}