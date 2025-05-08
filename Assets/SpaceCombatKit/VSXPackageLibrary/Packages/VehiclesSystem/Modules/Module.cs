using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using VSX.Utilities;

namespace VSX.Vehicles
{

    /// <summary>
    /// Unity event for running functions when the owner's root gameobject is set.
    /// </summary>
    [System.Serializable]
    public class OnSetRootTransformEventHandler : UnityEvent<Transform> { };


    /// <summary>
    /// This class represents a module that can be loaded or unloaded on a vehicle's module mount.
    /// </summary>
    public class Module : MonoBehaviour
    {

        protected Rigidbody m_Rigidbody;
        public Rigidbody Rigidbody { get { return m_Rigidbody; } }

        protected ModuleMount moduleMount;
        public ModuleMount ModuleMount { get { return moduleMount; } }

        [Header("General")]

        [SerializeField]
        protected List<string> labels = new List<string>();
        public List<string> Labels { get { return labels; } }

        [TextArea]
        [SerializeField]
        protected string description = "Module.";
        public string Description { get { return description; } }

        [SerializeField]
        protected string m_ID;
        public string ID { get { return m_ID; } }

        [SerializeField]
        protected List<Sprite> sprites = new List<Sprite>();
        public List<Sprite> Sprites { get { return sprites; } }

        [SerializeField]
        protected ModuleType moduleType;
        public ModuleType ModuleType { get { return moduleType; } }

        protected GameObject cachedGameObject;
        public GameObject CachedGameObject { get { return cachedGameObject; } }

        [Tooltip("If you have multiple parts on a module that need to be placed at the Attachment Points on a module mount, add them here.")]
        [SerializeField]
        protected List<Transform> attachmentItems = new List<Transform>();

        [Tooltip("Whether each of the Attachment Items should be parented to the corresponding Attachment Point on a module mount, rather than just being put in same position/rotation.")]
        [SerializeField]
        protected bool parentAttachmentItems = false;

        [Tooltip("Whether to set the layer of all children when the module layer is set (e.g. by the module mount).")]
        [SerializeField]
        protected bool setChildLayers = true;

        [Tooltip("Whether to deactivate the module's gameobject it is removed from a vehicle.")]
        [SerializeField]
        protected bool deactivateGameObjectOnRemovedFromMount;

        [Header("Mount Events")]

        public UnityEvent onMounted;

        public UnityEvent onUnmounted;

        public UnityEvent onActivated;

        public UnityEvent onDeactivated;

        protected bool isActivated = true;
        public bool IsActivated { get { return isActivated; } }

        // Module owner root gameobject set event
        public OnSetRootTransformEventHandler onSetRootTransform;
        protected List<IRootTransformUser> rootTransformUsers = new List<IRootTransformUser>();
        protected List<IGameAgentOwnable> gameAgentOwnables = new List<IGameAgentOwnable>();

        [Header("Ownership Events")]

        public UnityEvent onOwnedByPlayer;

        public UnityEvent onOwnedByAI;

        public UnityEvent onNoOwner;


        protected void Reset()
        {
            m_ID = UnityEngine.Random.Range(0, 1000000).ToString();
        }

        protected void Awake()
        {
            rootTransformUsers = new List<IRootTransformUser>(transform.GetComponentsInChildren<IRootTransformUser>());
            gameAgentOwnables = new List<IGameAgentOwnable>(transform.GetComponentsInChildren<IGameAgentOwnable>());

            m_Rigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Set the layer for the module.
        /// </summary>
        /// <param name="layer">The module layer.</param>
        public virtual void SetLayer(int layer)
        {
            if (setChildLayers)
            {
                Transform[] transforms = transform.GetComponentsInChildren<Transform>();
                foreach (Transform t in transforms)
                {
                    t.gameObject.layer = layer;
                }
            }
            else
            {
                transform.gameObject.layer = layer;
            }
        }

        public virtual void SetOwner(GameAgent gameAgent)
        {
            if (gameAgent == null)
            {
                OnNoOwner();
            }
            else
            {
                for (int i = 0; i < gameAgentOwnables.Count; ++i)
                {
                    gameAgentOwnables[i].Owner = gameAgent;
                }

                if (gameAgent.IsPlayer)
                {
                    OnOwnedByPlayer();
                }
                else
                {
                    OnOwnedByAI();
                }
            }
        }


        protected virtual void OnNoOwner()
        {
            onNoOwner.Invoke();
        }


        protected virtual void OnOwnedByPlayer()
        {
            onOwnedByPlayer.Invoke();
        }


        protected virtual void OnOwnedByAI()
        {
            onOwnedByAI.Invoke();
        }


        /// <summary>
        /// Called when this module is added to a module mount.
        /// </summary>
        /// <param name="moduleMount">The module mount this module was added to.</param>
        public virtual void OnAdded(ModuleMount moduleMount)
        {
            if (m_Rigidbody != null) m_Rigidbody.isKinematic = true;
            this.moduleMount = moduleMount;
        }


        /// <summary>
        /// Called when this module is removed from a module mount.
        /// </summary>
        /// <param name="moduleMount">The module mount this module was removed from.</param>
        public virtual void OnRemoved(ModuleMount moduleMount) 
        {
            if (m_Rigidbody != null) m_Rigidbody.isKinematic = false;
            transform.SetParent(null);
            this.moduleMount = null;
        }


        /// <summary>
        /// Called when this module is mounted at a module mount.
        /// </summary>
		public virtual void OnMounted()
        {
            if (moduleMount != null)
            {
                for (int i = 0; i < attachmentItems.Count; ++i)
                {
                    if (i < moduleMount.AttachmentPoints.Count)
                    {
                        attachmentItems[i].position = moduleMount.AttachmentPoints[i].position;
                        attachmentItems[i].rotation = moduleMount.AttachmentPoints[i].rotation;

                        if (parentAttachmentItems)
                        {
                            attachmentItems[i].transform.SetParent(moduleMount.AttachmentPoints[i]);
                        }
                    }
                }
            }

            onMounted.Invoke();
        }

        /// <summary>
        /// Called when this module is unmounted from a module mount.
        /// </summary>
		public virtual void OnUnmounted()
        {
            for (int i = 0; i < attachmentItems.Count; ++i)
            {
                attachmentItems[i].transform.SetParent(transform);
            }

            onUnmounted.Invoke();
        }

        /// <summary>
        /// Pass the module owner's root gameobject to relevant components via event.
        /// </summary>
        /// <param name="rootTransform">The owner's root gameobject.</param>
        public virtual void SetRootTransform(Transform rootTransform)
        {
            onSetRootTransform.Invoke(rootTransform);
            for (int i = 0; i < rootTransformUsers.Count; ++i)
            {
                rootTransformUsers[i].RootTransform = rootTransform;
            }
        }


        /// <summary>
        /// Called to activate or deactivate the module.
        /// </summary>
        /// <param name="activate">Whether the module is activated.</param>
        public virtual void SetActivated(bool activate)
        {
            if (activate && !isActivated)
            {
                onActivated.Invoke();
            }
            else if (!activate && isActivated)
            {
                onDeactivated.Invoke();
            }

            isActivated = activate;
        }
    }
}
