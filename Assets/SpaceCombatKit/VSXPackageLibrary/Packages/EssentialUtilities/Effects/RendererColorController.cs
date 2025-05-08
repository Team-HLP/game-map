using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    /// <summary>
    /// Stores a reference to a renderer and its color key, and makes it easy to modify its material properties.
    /// </summary>
    [System.Serializable]
    public class RendererColorController
    {

        public Renderer renderer;
        public int materialIndex = 0;
        public string colorID = "";
        public bool overrideHue = true;
        public bool overrideSaturation = true;
        public bool overrideValue = true;
        public bool preserveAlpha = true;

        protected float baseAlpha;


        public RendererColorController(Renderer renderer, string colorID = "")
        {
            this.renderer = renderer;
            this.colorID = colorID;

            Initialize();
        }


        public virtual void Initialize()
        {
            baseAlpha = GetColor().a;
        }


        /// <summary>
        /// Set the alpha of this animated renderer. This will completely override the current value.
        /// </summary>
        /// <param name="alpha">The alpha for this renderer.</param>
        public virtual void SetAlpha(float alpha)
        {
            if (colorID != "")
            {
                Color c = renderer.materials[materialIndex].GetColor(colorID);
                c.a = alpha;
                renderer.materials[materialIndex].SetColor(colorID, c);
            }
            else
            {
                Color c = renderer.material.color;
                c.a = alpha;
                renderer.material.color = c;
            }
        }


        public virtual void ApplyAlpha(float alpha)
        {
            SetAlpha(preserveAlpha ? baseAlpha * alpha : alpha);
        }


        /// <summary>
        /// Get the color of this animated renderer.
        /// </summary>
        /// <returns>The color.</returns>
        public virtual Color GetColor()
        {
            if (colorID != "")
            {
                return renderer.material.GetColor(colorID);
            }
            else
            {
                return renderer.material.color;
            }
        }


        public virtual void ApplyColor(Color c)
        {
            float hueOverrideValue, saturationOverrideValue, valueOverrideValue;
            Color.RGBToHSV(c, out hueOverrideValue, out saturationOverrideValue, out valueOverrideValue);

            float alpha = GetColor().a;
            float h, s, v;
            Color.RGBToHSV(GetColor(), out h, out s, out v);
            if (overrideHue) h = hueOverrideValue;
            if (overrideSaturation) s = saturationOverrideValue;
            if (overrideValue) v = valueOverrideValue;
            c = Color.HSVToRGB(h, s, v, true);
            c.a = alpha;

            SetColor(c);
        }


        /// <summary>
        /// Set the color of this animated renderer. This will completely override the current value.
        /// </summary>
        /// <param name="newColor">The new color.</param>
        public virtual void SetColor(Color newColor)
        {
            if (colorID != "")
            {
                renderer.material.SetColor(colorID, newColor);
            }
            else
            {
                renderer.material.color = newColor;
            }
        }
    }
}
