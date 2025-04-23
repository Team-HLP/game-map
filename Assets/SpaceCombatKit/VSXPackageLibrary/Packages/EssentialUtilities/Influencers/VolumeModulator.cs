using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    /// <summary>
    /// Generate a modulation value when this transform is inside a relevant volume.
    /// </summary>
    public class VolumeModulator : Modulator
    {

        [Tooltip("The modulation value that is applied when this transform is inside a volume on one of the specified layers.")]
        [SerializeField]
        protected float modulationValue = 0.2f;


        [Tooltip("The layer(s) that a volume must be on to affect the modulation value.")]
        [SerializeField]
        protected LayerMask volumeLayers;


        public override float Value()
        {
            float val = VolumeManager.Instance == null ? 0 : VolumeManager.Instance.GetValue(transform.position, volumeLayers);
            return val * modulationValue + (1 - val);
        }
    }
}
