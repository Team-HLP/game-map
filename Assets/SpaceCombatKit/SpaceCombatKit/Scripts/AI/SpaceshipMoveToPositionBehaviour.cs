﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Vehicles;
using VSX.Engines3D;

namespace VSX.SpaceCombatKit
{
    public class SpaceshipMoveToPositionBehaviour : AISpaceshipBehaviour
    {

        [Tooltip("The target position to move toward.")]
        [SerializeField]
        protected Vector3 targetPosition;

        protected override bool Initialize(Vehicle vehicle)
        {

            if (!base.Initialize(vehicle)) return false;

            engines = vehicle.GetComponent<VehicleEngines3D>();
            if (engines == null) return false;

            return true;
           
        }

        public override bool BehaviourUpdate()
        {

            if (!base.BehaviourUpdate()) return false;

            // Steer
            Maneuvring.TurnToward(vehicle.transform, targetPosition, maxRotationAngles, shipPIDController.steeringPIDController);
            engines.SetSteeringInputs(shipPIDController.GetSteeringControlValues());

            // Move
            Maneuvring.TranslateToward(engines.Rigidbody, targetPosition, shipPIDController.movementPIDController);
            engines.SetMovementInputs(shipPIDController.GetMovementControlValues());

            return true;
            
        }
    }
}
