using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Vehicles;
using VSX.Utilities;

namespace VSX.HUD
{
    /// <summary>
    /// Manages all the components of the HUD.
    /// </summary>
    [DefaultExecutionOrder(30)]
    public class HUDManager : MonoBehaviour, ICamerasUser, IGameAgentOwnable
    {
        [Tooltip("The index of the HUD camera within the list of cameras (passed by the Camera Entity).")]
        [SerializeField]
        protected int HUDCameraIndex = 0;

        protected List<HUDComponent> hudComponents = new List<HUDComponent>();

        protected bool activated = false;
        public virtual bool Activated { get { return activated; } }

        [Tooltip("Whether to activate the HUD when the scene starts.")]
        [SerializeField]
        protected bool activateOnStart = false;

        [Tooltip("Whether to activate the HUD when the player enters.")]
        [SerializeField]
        protected bool activateOnPlayerEnter = true;

        protected IHUDCameraUser[] m_HUDCameraUsers;

        protected GameAgent owner;
        public GameAgent Owner
        {
            get => owner;
            set
            {
                owner = value;

                if (activateOnPlayerEnter)
                {
                    if (owner != null && owner.IsPlayer)
                    {
                        ActivateHUD();
                    }
                    else
                    {
                        DeactivateHUD();
                    }
                }
            }
        }




        protected virtual void Awake()
        {

            hudComponents = new List<HUDComponent>(transform.GetComponentsInChildren<HUDComponent>(true));
            m_HUDCameraUsers = transform.GetComponentsInChildren<IHUDCameraUser>();

            Vehicle vehicle = transform.GetComponent<Vehicle>();
            if (vehicle != null)
            {
                vehicle.onDestroyed.AddListener(DeactivateHUD);
            }
        }


        /// <summary>
        /// Pass the list of cameras to the HUD.
        /// </summary>
        /// <param name="cameras">The list of cameras.</param>
        public virtual void SetCameras(List<Camera> cameras)
        {
            if (cameras.Count > HUDCameraIndex)
            {
                for (int i = 0; i < m_HUDCameraUsers.Length; ++i)
                {
                    m_HUDCameraUsers[i].HUDCamera = cameras[HUDCameraIndex];
                }
            }
        }


        // Called when the scene starts
        protected virtual void Start()
        {
            if (!activated)
            {
                if (activateOnStart)
                {
                    ActivateHUD();
                }
                else
                {
                    if (owner == null)
                    {
                        DeactivateHUD();
                    }
                }
            }
        }


        /// <summary>
        /// Set the camera for the HUD.
        /// </summary>
        /// <param name="hudCamera">The HUD camera.</param>
        public virtual void SetHUDCamera(Camera hudCamera)
        {
            for (int i = 0; i < m_HUDCameraUsers.Length; ++i)
            {
                m_HUDCameraUsers[i].HUDCamera = hudCamera;
            }
        }


        /// <summary>
        /// Activate the HUD.
        /// </summary>
        public virtual void ActivateHUD()
        {
            for (int i = 0; i < hudComponents.Count; ++i)
            {
                if (hudComponents[i] != null && !hudComponents[i].Unplugged)
                {
                    hudComponents[i].Activate();
                }
            }

            activated = true;
        }


        /// <summary>
        /// Deactivate the HUD.
        /// </summary>
        public virtual void DeactivateHUD()
        {
            for (int i = 0; i < hudComponents.Count; ++i)
            {
                if (hudComponents[i] != null && !hudComponents[i].Unplugged)   // Necessary because when OnDisable is called when scene is being destroyed, not checking causes error
                {
                    hudComponents[i].Deactivate();
                }
            }

            activated = false;
        }


        // Called every frame
        public virtual void LateUpdate()
        {
            if (activated)
            {
                for (int i = 0; i < hudComponents.Count; ++i)
                {
                    if (hudComponents[i].Activated)
                    {
                        hudComponents[i].OnUpdateHUD();
                    }
                    else
                    {
                        if (!hudComponents[i].Unplugged)
                        {
                            hudComponents[i].Activate();
                        }
                    }
                }
            }
        }
    }
}
