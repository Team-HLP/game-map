using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Loadouts;
using VSX.Vehicles;

namespace VSX.SpaceCombatKit
{
    public class LoadoutShipSpawner : LoadoutVehicleSpawner
    {
        public bool startLanded = true;


        protected virtual void Awake()
        {
            onVehicleSpawned.AddListener(OnVehicleSpawned);
        }

        void OnVehicleSpawned(Vehicle vehicle)
        {
            ShipLander lander = vehicle.GetComponent<ShipLander>();
            if (lander != null)
            {
                lander.StartLanded = startLanded;
            }
        }
    }
}

