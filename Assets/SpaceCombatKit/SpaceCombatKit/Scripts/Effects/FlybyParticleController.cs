using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VSX.Utilities;
using VSX.Vehicles;
using VSX.VehicleCombatKits;

namespace VSX.SpaceCombatKit
{

    /// <summary>
    /// This class manages the flyby particles effects to help the player to feel the speed of the vehicle.
    /// </summary>
    public class FlybyParticleController : MonoBehaviour 
	{

        public VehicleCamera vehicleCamera;

        [SerializeField]
		protected ParticleSystem flybyParticleSystem;
        protected ParticleSystem.MainModule flybyParticleSystemMainModule;
        protected ParticleSystemRenderer flybyParticleSystemRenderer;

        [SerializeField]
        protected Color particleColor = Color.white;

        [SerializeField]
        protected bool velocityBasedSize = true;

		[SerializeField]
        protected float velocityToSizeCoefficient = 0.005f;

        [SerializeField]
        protected float maxSize = 1000;

        [SerializeField]
        protected bool velocityBasedAlpha = true;
	
		[SerializeField]
        protected float velocityToAlphaCoefficient = 0.005f;

        [SerializeField]
        protected bool velocityBasedLengthScale = false;

        [SerializeField]
        protected float velocityToLengthScaleCoefficient = 1;

        [SerializeField]
        protected float maxLengthScale = 5;

        [SerializeField]
        protected float maxAlpha = 1;

        [SerializeField]
        protected float frontOffset = 50;

        [SerializeField]
        protected float leadDistance = 200;

        public List<Modulator> sizeModulators = new List<Modulator>();
        public List<Modulator> alphaModulators = new List<Modulator>();

      
        protected virtual void Awake()
		{
			flybyParticleSystemMainModule = flybyParticleSystem.main;
            flybyParticleSystemRenderer = flybyParticleSystem.GetComponent<ParticleSystemRenderer>();

            Color c = particleColor;

            if (velocityBasedAlpha)
            {
                c.a = 0;
            }

            flybyParticleSystemMainModule.startColor = c;
		}


        /// <summary>
        /// Update the flyby particle effects according to the velocity of the vehicle.
        /// </summary>
        /// <param name="vehicleVelocity">The velocity of the vehicle.</param>
        public virtual void UpdateEffect(Vehicle vehicle)
		{
            if (vehicle.CachedRigidbody == null) return;

            float size = flybyParticleSystemMainModule.startSize.constant;
            if (velocityBasedSize)
            {
                size = Mathf.Clamp(vehicle.CachedRigidbody.velocity.magnitude * velocityToSizeCoefficient, 0, maxSize);
            }
            for(int i = 0; i < sizeModulators.Count; ++i)
            {
                size *= sizeModulators[i].Value();
            }
            flybyParticleSystemMainModule.startSize = size;

            float lengthScale = flybyParticleSystemRenderer.lengthScale;
            if (velocityBasedLengthScale)
            {
                lengthScale = Mathf.Clamp(vehicle.CachedRigidbody.velocity.magnitude * velocityToLengthScaleCoefficient, 0, maxLengthScale);
            }
            flybyParticleSystemRenderer.lengthScale = lengthScale;

            Color c = particleColor;
            float alpha = c.a;
            if (velocityBasedAlpha)
            {
                alpha = Mathf.Clamp(vehicle.CachedRigidbody.velocity.magnitude * velocityToAlphaCoefficient, 0, maxAlpha);
            }
            for (int i = 0; i < alphaModulators.Count; ++i)
            {
                alpha *= alphaModulators[i].Value();
            }
            c.a = alpha;

            flybyParticleSystemMainModule.startColor = c;

            ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = flybyParticleSystem.velocityOverLifetime;
            velocityOverLifetimeModule.x = -vehicle.CachedRigidbody.velocity.x;
            velocityOverLifetimeModule.y = -vehicle.CachedRigidbody.velocity.y;
            velocityOverLifetimeModule.z = -vehicle.CachedRigidbody.velocity.z;

            
            if (vehicle.CachedRigidbody.velocity != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(vehicle.CachedRigidbody.velocity.normalized, vehicle.transform.up);
                transform.position = vehicle.CachedRigidbody.transform.position + vehicle.transform.forward * frontOffset + vehicle.CachedRigidbody.velocity.normalized * leadDistance;
            }
            
        }


        private void Update()
        {
            if (vehicleCamera.TargetVehicle != null)
            {
                UpdateEffect(vehicleCamera.TargetVehicle);
            }
        }
    }
}
