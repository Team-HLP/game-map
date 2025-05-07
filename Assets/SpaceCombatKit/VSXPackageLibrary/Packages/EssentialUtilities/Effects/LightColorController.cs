using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    [System.Serializable]
    public class LightColorController
    {
        [SerializeField]
        protected Light light;
        public virtual Light Light { get => light; set => light = value; }

        [SerializeField]
        protected bool applyHue = true;
        public virtual bool ApplyHue { get => applyHue; set => applyHue = value; }

        [SerializeField]
        protected bool applySaturation = true;
        public virtual bool ApplySaturation { get => applySaturation; set => applySaturation = value; }

        [SerializeField]
        protected bool applyValue = true;
        public virtual bool ApplyValue { get => applyValue; set => applyValue = value; }

        protected float baseIntensity;
        public virtual float BaseIntensity { get => baseIntensity; }


        public virtual void Initialize()
        {
            baseIntensity = light.intensity;
        }


        public virtual void ApplyColor (Color color)
        {
            float h, s, v;
            Color.RGBToHSV(light.color, out h, out s, out v);

            float h_Applied, s_Applied, v_Applied;
            Color.RGBToHSV(color, out h_Applied, out s_Applied, out v_Applied);

            if (applyHue)
            {
                h = h_Applied;
            }

            if (applySaturation)
            {
                s = s_Applied;
            }

            if (applyValue)
            {
                v = v_Applied;
            }

            light.color = Color.HSVToRGB(h, s, v);
        }


        public virtual void SetIntensityLevel(float level)
        {
            light.intensity = baseIntensity * level;
        }
    }
}
