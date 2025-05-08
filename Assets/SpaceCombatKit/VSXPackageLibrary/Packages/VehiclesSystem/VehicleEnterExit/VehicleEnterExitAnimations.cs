using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Vehicles
{
    /// <summary>
    /// Play animations for entering/exiting a vehicle.
    /// </summary>
    public class VehicleEnterExitAnimations : MonoBehaviour
    {
        /// <summary>
        /// Begin animation for moving from one vehicle to another.
        /// </summary>
        /// <param name="agent">The game agent involved.</param>
        /// <param name="from">The vehicle being exited.</param>
        /// <param name="to">The vehicle being entered.</param>
        public virtual void Animate(GameAgent agent, Vehicle fromVehicle, Vehicle toVehicle) { }
    }
}

