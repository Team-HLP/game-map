using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.CameraSystem;

namespace VSX.Utilities
{
    /// <summary>
    /// An aim controller that provides aim information to other components.
    /// </summary>
    [DefaultExecutionOrder(10)]
    public class AimController : AimControllerBase, ICamerasUser
    {
        [Tooltip("The transform representing the position the aim starts from.")]
        [SerializeField]
        protected Transform aimOrigin;

        [Tooltip("The cursor to get the aim direction from.")]
        [SerializeField]
        protected CustomCursor cursor;

        [Tooltip("The camera target of the object or vehicle doing the aiming.")]
        [SerializeField]
        protected CameraTarget cameraTarget;

        [Tooltip("Whether to use the camera following the Camera Target as the aim origin.")]
        [SerializeField]
        protected bool useCameraAsAimOrigin = true;

        [Tooltip("Whether to raycast along the aim direction and aim at the hit point.")]
        [SerializeField]
        protected bool raycastAim = true;
        public bool RaycastAim
        {
            get { return raycastAim; }
            set { raycastAim = value; }
        }

        [Tooltip("The layer mask used for raycast aiming.")]
        [SerializeField]
        protected LayerMask raycastAimMask;

        [Tooltip("Whether to ignore trigger colliders for the aim raycast.")]
        [SerializeField]
        protected bool ignoreTriggerColliders = true;

        protected RaycastHitComparer raycastHitComparer;    // Used to sort hits by distance


        protected virtual void Reset()
        {
            cameraTarget = GetComponent<CameraTarget>();

            cursor = transform.GetComponentInChildren<CustomCursor>();

            aimOrigin = transform;

            raycastAimMask = ~0;
        }


        protected virtual void Awake()
        {
            raycastHitComparer = new RaycastHitComparer();

            if (aimOrigin == null) aimOrigin = transform;
        }


        /// <summary>
        /// Set the cameras to get camera information from.
        /// </summary>
        /// <param name="cameras">The cameras list.</param>
        public virtual void SetCameras(List<Camera> cameras)
        {
            if (useCameraAsAimOrigin && cameras.Count > 0) aimOrigin = cameras[0].transform;
        }


        /// <summary>
        /// Check for hits along the aim direction using a raycast.
        /// </summary>
        public virtual void RaycastAimUpdate()
        {
            aim.origin = aimOrigin.position;
            aim.direction = aimOrigin.forward;

            if (cursor != null) aim.direction = cursor.AimDirection;

            // Get all raycast hits
            RaycastHit[] hits = Physics.RaycastAll(aim, 1000000, raycastAimMask, ignoreTriggerColliders ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide);

            // Sort hits by distance
            System.Array.Sort(hits, raycastHitComparer);

            // Discard hits on self and find one that is valid
            hitFound = false;
            for (int i = 0; i < hits.Length; ++i)
            {
                if (hits[i].collider.transform.IsChildOf(transform))
                {
                    continue;
                }

                hitFound = true;
                hit = hits[i];

                break;
            }
        }


        // Called every frame
        protected virtual void LateUpdate()
        {
            if (raycastAim) RaycastAimUpdate();
        }
    }
}
