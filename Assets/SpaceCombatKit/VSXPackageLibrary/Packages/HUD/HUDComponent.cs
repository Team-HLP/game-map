using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.HUD
{

    /// <summary>
    /// Base class for a script that controls a specific part of the HUD.
    /// </summary>
    public class HUDComponent : MonoBehaviour, IHUDCameraUser
    {

        [Header("HUD Component")]

        [Tooltip("When checked, this HUD component will ignore activation/deactivation commands from the HUD Manager. Useful for temporarily disabling parts of the HUD.")]
        [SerializeField]
        protected bool unplugged = false;
        public bool Unplugged
        {
            get { return unplugged; }
            set
            {
                unplugged = value;
                if (activated) Deactivate();
            }
        }

        [Tooltip("The camera that is displaying this HUD component.")]
        [SerializeField]
        protected Camera m_HUDCamera;
        public Camera HUDCamera
        {
            get { return m_HUDCamera; }
            set
            {
                m_HUDCamera = value;
                foreach (Canvas canvas in canvases)
                {
                    canvas.worldCamera = m_HUDCamera;
                }
            }
        }

        protected List<Canvas> canvases = new List<Canvas>();

        [Tooltip("Whether to activate this HUD component when the scene starts.")]
        [SerializeField]
        protected bool activateOnAwake = false;

        [Tooltip("Whether to update this HUD component every frame on its own. Used when it is not being managed by a HUD Manager component.")]
        [SerializeField]
        protected bool independentUpdate = false;

        protected bool activated = false;
        public virtual bool Activated { get { return activated; } }


        protected virtual void Awake()
        {
            if (activateOnAwake)
            {
                Activate();
            }

            canvases = new List<Canvas>(GetComponentsInChildren<Canvas>(true));
        }


        /// <summary>
        /// Activate this HUD Component
        /// </summary>
        public virtual void Activate()
        {
            gameObject.SetActive(true);
            activated = true;
        }


        /// <summary>
        /// Deactivate this HUD component
        /// </summary>
        public virtual void Deactivate()
        {
            gameObject.SetActive(false);
            activated = false;
        }


        /// <summary>
        /// Parent this HUD component to a specified transform.
        /// </summary>
        /// <param name="parentTransform">The parent transform.</param>
        public virtual void ParentToTransform(Transform parentTransform)
        {
            transform.SetParent(parentTransform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }


        /// <summary>
        /// Clear the parent of this HUD component.
        /// </summary>
        public virtual void ClearParent()
        {
            transform.parent = null;
        }


        /// <summary>
        /// Called to update this HUD Component.
        /// </summary>
        public virtual void OnUpdateHUD() { }


        // Called every frame
        protected virtual void Update()
        {
            if (independentUpdate && activated)
            {
                OnUpdateHUD();
            }
        }
    }
}