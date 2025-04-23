using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.AI;
using VSX.Engines3D;
using VSX.Vehicles;

namespace VSX.SpaceCombatKit
{
    public class AISpaceshipBehaviour : AIVehicleBehaviour
    {

        [SerializeField]
        protected ShipPIDController shipPIDController;

        [SerializeField]
        protected Vector3 maxRotationAngles = new Vector3(360, 360, 360);

        protected VehicleEngines3D engines;


        protected override bool Initialize(Vehicle vehicle)
        {
            if (!base.Initialize(vehicle)) return false;

            engines = vehicle.GetComponent<VehicleEngines3D>();
            if (engines == null) { return false; }

            return true;

        }


        protected virtual void SteerToward(Vector3 steeringTarget) 
        {
            Maneuvring.TurnToward(vehicle.transform, steeringTarget, maxRotationAngles, shipPIDController.steeringPIDController);
            engines.SetSteeringInputs(shipPIDController.GetSteeringControlValues());
        }

        protected virtual void MoveToward(Vector3 moveTarget) 
        {
            Maneuvring.TranslateToward(engines.Rigidbody, moveTarget, shipPIDController.movementPIDController);
            engines.SetMovementInputs(shipPIDController.GetMovementControlValues());
        }
    }
}