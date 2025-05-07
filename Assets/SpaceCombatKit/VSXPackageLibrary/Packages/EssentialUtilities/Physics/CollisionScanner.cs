using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace VSX.Utilities
{
    /// <summary>
    /// Unity event for running functions when a raycast hit is detected
    /// </summary>
    [System.Serializable]
    public class OnCollisionScannerHitDetectedEventHandler : UnityEvent<RaycastHit> { }

    /// <summary>
    /// This class uses a raycast from the transform's previous position to its current one to detect a hit on a collider regardless of speed.
    /// </summary>
    public class CollisionScanner : MonoBehaviour, IRootTransformUser
    {

        [Header("Settings")]

        [SerializeField]
        protected LayerMask hitMask = Physics.DefaultRaycastLayers;
        public LayerMask HitMask { get { return hitMask; } }

        [SerializeField]
        protected HitScanIntervalType hitScanIntervalType = HitScanIntervalType.FrameInterval;

        // Frame interval

        [SerializeField]
        protected int hitScanFrameInterval = 1;
        protected int frameCountSinceLastScan = 1;

        // Time interval

        [SerializeField]
        protected float hitScanTimeInterval;
        protected float lastHitScanTime;

        protected Vector3 lastPosition;

        [Tooltip("Whether to ignore trigger colliders when scanning for collisions.")]
        [SerializeField]
        protected bool ignoreTriggerColliders = false;

        [Tooltip("Whether to ignore collision with the object or vehicle that this object came from.")]
        [SerializeField]
        protected bool ignoreHierarchyCollision = true;

        [SerializeField]
        protected Transform rootTransform;    // To check for collisions with firer
        public Transform RootTransform { set { rootTransform = value; } }

        [SerializeField]
        protected Rigidbody m_Rigidbody;
        public Rigidbody Rigidbody
        {
            get { return m_Rigidbody; }
            set { m_Rigidbody = value; }
        }

        [Header("Events")]

        // Hit detected event
        public OnCollisionScannerHitDetectedEventHandler onHitDetected;
        
        protected bool disabled = false;


        protected virtual void Reset()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        // Reset when enabled
        private void OnEnable()
        {
            disabled = false;
            UpdateLastPosition();
        }

     
        // Do a single hit scan
        protected void DoHitScan()
        {

            if (disabled) return;

            RaycastHit[] hits;

            // Scan from previous position to current position
            float scanDistance = Vector3.Distance(lastPosition, transform.position);
            
            // Raycast
            hits = Physics.RaycastAll(lastPosition, transform.forward, scanDistance, hitMask, ignoreTriggerColliders ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));    // Sort by distance

            for (int i = 0; i < hits.Length; ++i)
            {
                if (ignoreHierarchyCollision && hits[i].transform.IsChildOf(rootTransform))
                {
                    continue;
                }
                
                transform.position = hits[i].point;
                disabled = true;
                onHitDetected.Invoke(hits[i]);

                break;

            }

            UpdateLastPosition();

        }

        /// <summary>
        /// Disable this collision scanner.
        /// </summary>
        public void SetHitScanDisabled()
        {
            disabled = true;
        }

        /// <summary>
        /// Enable this collision scanner
        /// </summary>
        public void SetHitScanEnabled()
        {
            disabled = false;
        }


        public virtual void UpdateLastPosition()
        {
            // Update the last position
            lastPosition = transform.position;
        }


        // Called every frame
        private void Update()
        {

            switch (hitScanIntervalType)
            {
                case HitScanIntervalType.FrameInterval:

                    // Check if enough frames have passed for a new scan
                    if (frameCountSinceLastScan >= hitScanFrameInterval)
                    {
                        DoHitScan();
                        frameCountSinceLastScan = 0;
                    }

                    break;

                case HitScanIntervalType.TimeInterval:

                    // Check if enough time has passed for a new scan
                    if ((Time.time - lastHitScanTime) > hitScanTimeInterval)
                    {
                        DoHitScan();
                        lastHitScanTime = Time.time;
                    }

                    break;

            }

            frameCountSinceLastScan += 1;

        }
    }
}