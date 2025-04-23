using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Utilities;


namespace VSX.Weapons
{
    /// <summary>
    /// Control visual effects for a beam weapon unit.
    /// </summary>
    public class BeamVisualEffectsController : VisualEffectsController
    {

        [Tooltip("The beam weapon unit to show effects for.")]
        [SerializeField]
        protected BeamWeaponUnit beamWeaponUnit;


        /// <summary>
        /// Called when this component is first added to a gameobject or reset in the inspector.
        /// </summary>
        protected virtual void Reset()
        {
            beamWeaponUnit = GetComponentInChildren<BeamWeaponUnit>();
        }


        protected override void Awake()
        {
            base.Awake();

            beamWeaponUnit.onBeamLevelSet.AddListener(SetLevel);
        }
    }
}
