using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace VSX.Effects
{

	/// <summary>
    /// Creates a flash by modifying the alpha of a set of materials over time.
    /// </summary>
	public class FlashController : MonoBehaviour 
	{   
        [Header("Settings")]

        [Tooltip("How long a single flash takes.")]
        [SerializeField]
        protected float flashPeriod = 0.1f;

        [Tooltip("A curve describing the material alpha over the course of a single flash.")]
        [SerializeField]
        protected AnimationCurve alphaOverLifetimeCurve = AnimationCurve.Linear(0, 1, 1, 0);

        protected float flashStartTime;
        protected float curveLocation = 0;
        protected float flashLevel = 0;
        protected bool flashStarted = false;

        [Header("VFX Elements")]

        [Tooltip("The materials to control the alpha for during the flash.")]
        [SerializeField]
        protected List<Renderer> vfxRenderers = new List<Renderer>();
        protected List<Material> vfxMaterials = new List<Material>();

        [Tooltip("Whether to use a specific color ID when setting the material alpha (if unchecked, Material.color is used).")]
        [SerializeField]
        protected bool overrideMaterialColorID = false;

        [Tooltip("if 'Override Material Color ID' is checked, this color ID will be used when setting the materials alpha.")]
        [SerializeField]
        protected string materialColorIDOverride = "";

        [Header("Events")]

        [Tooltip("Event called when a flash occurs.")]
        // Called whenever a flash begins
        public UnityEvent onFlash;



        protected virtual void Awake()
		{          
            // Cache the materials
			for (int i = 0; i < vfxRenderers.Count; ++i)
            {
                vfxMaterials.Add(vfxRenderers[i].material);
            }
            
            // Prevent flash at start
            flashStartTime = -flashPeriod;

            SetLevel(0);           
		}
	
		
        /// <summary>
        /// Create a flash.
        /// </summary>
		public virtual void Flash()
		{
            flashStarted = true;
            flashStartTime = Time.time;

            onFlash.Invoke();
		}
        

        /// <summary>
        /// Set the flash level directly.
        /// </summary>
        /// <param name="newLevel">The new flash level.</param>
        public virtual void SetLevel(float newLevel)
        {
            // Clamp from 0 to 1
            flashLevel = Mathf.Clamp(newLevel, 0, 1);
            
            // Set the flash alpha
            for (int i = 0; i < vfxRenderers.Count; ++i)
            {
                Color c;
                if (overrideMaterialColorID)
                {
                    c = vfxMaterials[i].GetColor(materialColorIDOverride);
                }
                else
                {
                    c = vfxMaterials[i].color;
                }

                c.a = newLevel;

                if (overrideMaterialColorID)
                {
                    vfxMaterials[i].SetColor(materialColorIDOverride, c);
                }
                else
                {
                    vfxMaterials[i].color = c;
                }
            }
        }

        
        // Called every frame
		protected virtual void Update()
		{
            
            if (flashStarted)
            {
                // Prevent divide-by-zero errors with zero flash period
                if (Mathf.Approximately(flashPeriod, 0))
                {
                    flashStarted = false;
                    curveLocation = 1;
                }
                else
                {
                    // Calculate flash level from curve
                    curveLocation = (Time.time - flashStartTime) / flashPeriod;
                    if (curveLocation > 1)
                    {
                        curveLocation = 1;
                        flashStarted = false;
                    }
                }

                // Set flash level
                SetLevel(alphaOverLifetimeCurve.Evaluate(curveLocation));
            }
        }
	}
}
