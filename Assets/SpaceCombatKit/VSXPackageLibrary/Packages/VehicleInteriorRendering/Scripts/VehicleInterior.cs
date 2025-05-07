using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.Vehicles.InteriorRendering
{
    public class VehicleInterior : MonoBehaviour
    {

        [Tooltip("The vehicle that this interior is for.")]
        [SerializeField]
        protected Vehicle vehicle;
        public Vehicle Vehicle { get { return vehicle; } }


        protected List<VehicleInteriorVolume> volumes = new List<VehicleInteriorVolume>();
        public List<VehicleInteriorVolume> Volumes { get { return volumes; } }


        [Tooltip("Whether to disable the renderers on this vehicle interior when the player is inside.")]
        [SerializeField]
        protected bool disableRenderers = true;


        [Tooltip("Whether to deactivate this gameobject when the player exits the vehicle.")]
        [SerializeField]
        protected bool disableGameObjectOnPlayerExited = true;

        public UnityEvent onPlayerEntered;
        public UnityEvent onPlayerExited;

        protected List<Renderer> renderers = new List<Renderer>();


        protected virtual void Awake()
        {
            volumes = new List<VehicleInteriorVolume>(GetComponentsInChildren<VehicleInteriorVolume>());
            renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
        }


        /// <summary>
        /// Make sure every child object of this gameobject's is on the interior layer mask. 
        /// </summary>
        /// <param name="layerMask">The layer mask.</param>
        public virtual void CheckLayers(LayerMask layerMask)
        {
            CheckLayersRecursive(transform, layerMask);
        }


        /// <summary>
        /// Recursively check each child of a transform to make sure it is on the interior layer mask.
        /// </summary>
        /// <param name="t">The transform to check.</param>
        /// <param name="layerMask">The layermask.</param>
        protected virtual void CheckLayersRecursive(Transform t, LayerMask layerMask)
        {
            if (!(layerMask == (layerMask | (1 << t.gameObject.layer))))
            {
                Debug.LogWarning("Game Object found on interior of vehicle " + vehicle.name + " which does not belong to the interior layer mask. This may cause problems.");
            }
        }


        /// <summary>
        /// Called when the player enters this vehicle interior.
        /// </summary>
        public virtual void OnPlayerEntered()
        {
            if (transform.IsChildOf(vehicle.transform)) transform.SetParent(null);
            transform.position = InteriorRenderingManager.Instance.InteriorSpawnPosition;

            gameObject.SetActive(true);

            if (disableRenderers)
            {
                foreach (Renderer renderer in renderers)
                {
                    renderer.enabled = false;
                }
            }

            onPlayerEntered.Invoke();
        }


        /// <summary>
        /// Called when the player exits this vehicle interior.
        /// </summary>
        public virtual void OnPlayerExited()
        {
            if (disableGameObjectOnPlayerExited)
            {
                gameObject.SetActive(false);
            }

            onPlayerExited.Invoke();
        }
    }
}
