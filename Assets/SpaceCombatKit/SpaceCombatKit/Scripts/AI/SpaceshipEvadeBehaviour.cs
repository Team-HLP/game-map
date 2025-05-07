using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Vehicles;
using VSX.Weapons;
using VSX.Engines3D;
using VSX.AI;

namespace VSX.SpaceCombatKit
{
    public class SpaceshipEvadeBehaviour : AISpaceshipBehaviour
    {
        [SerializeField]
        [Range(0, 1)]
        protected float turnAwayFromTarget = 0.5f;

        [SerializeField]
        [Range(0, 1)]
        protected float verticalAmount = 0.5f;

        [SerializeField]
        protected float range = 3000;

        [SerializeField]
        protected float turnDegrees = 120;

        protected Vector3 evadeDirection = Vector3.forward;

        protected WeaponsController weapons;

        public float minChangeDirectionInterval = 1;
        public float maxChangeDirectionInterval = 4;
        float nextChangeDirectionInterval;
        float changeDirectionStartTime;

        

        protected override bool Initialize(Vehicle vehicle)
        {

            if (!base.Initialize(vehicle)) return false; 

            weapons = vehicle.GetComponent<WeaponsController>();
            if (weapons == null) { return false; }
            
            engines = vehicle.GetComponent<VehicleEngines3D>();
            if (engines == null) { return false; }
            
            return true;

        }


        public override void StartBehaviour()
        {
            base.StartBehaviour();

            if (state == VehicleBehaviourState.Started)
            {
                UpdateEvadeDirection();
            }
        }


        public virtual void UpdateEvadeDirection()
        {

            if (weapons.WeaponsTargetSelector == null || weapons.WeaponsTargetSelector.SelectedTarget == null) return;
            
            // Get the direction to the target
            Vector3 toTargetDirection = (weapons.WeaponsTargetSelector.SelectedTarget.transform.position - vehicle.transform.position).normalized;

            // Get a new evade path direction
            evadeDirection = Quaternion.Euler(Random.Range(-30, 30), turnDegrees - Random.Range(0, 2) * turnDegrees * 2, 0) * evadeDirection;

            evadeDirection = Vector3.Slerp(evadeDirection, -toTargetDirection, turnAwayFromTarget);

            Vector3 flattenedEvade = new Vector3(evadeDirection.x, 0, evadeDirection.z).normalized;
            evadeDirection = Vector3.Slerp(evadeDirection, flattenedEvade, 1 - verticalAmount);
            
            // Get the vector from the vehicle to the scene center
            //Vector3 toCenterVec = -vehicle.transform.position;
            
            // Blend with a return to center vector depending on distance from center
            //float returnToCenterStrength = Mathf.Clamp((toCenterVec.magnitude / (range * 2)), 0f, 1f);
            //evadeDirection = (returnToCenterStrength * toCenterVec.normalized + (1 - returnToCenterStrength) * evadeDirection.normalized).normalized;

            nextChangeDirectionInterval = Random.Range(minChangeDirectionInterval, maxChangeDirectionInterval);
            changeDirectionStartTime = Time.time;

            Debug.DrawLine(vehicle.transform.position, vehicle.transform.position + evadeDirection * 200, Color.yellow);
        }

    
        // Add a weave to the path of the vehicle
        protected virtual Vector3 Weave(Vector3 pathDirection, float weaveSpeed, float weaveRadius)
        {

            // Add relative horizontal offset
            float offsetX = (Mathf.PerlinNoise(Time.time * weaveSpeed, 0f) - 0.5f) * 2f;

            // Add relative vertical offset
            float offsetY = (Mathf.PerlinNoise(0f, Time.time * weaveSpeed) - 0.5f) * 2f;

            // Create the offset vector
            Vector3 offsetVec = new Vector3(offsetX, offsetY, 1).normalized * weaveRadius;
            Vector3 offset = new Vector3(offsetVec.x, offsetVec.y, 1).normalized;

            pathDirection = Quaternion.FromToRotation(Vector3.forward, pathDirection) * offset;

            return pathDirection;

        }


        public override bool BehaviourUpdate()
        {
            if (!base.BehaviourUpdate()) return false;
            
            if (Time.time - changeDirectionStartTime >= nextChangeDirectionInterval)
            {
                UpdateEvadeDirection();
            }

            Maneuvring.TurnToward(vehicle.transform, vehicle.transform.position + evadeDirection, maxRotationAngles, shipPIDController.steeringPIDController);
            engines.SetSteeringInputs(shipPIDController.steeringPIDController.GetControlValues());

            engines.SetMovementInputs(new Vector3(0, 0, 1));

            return true;
            
        }
    }
}