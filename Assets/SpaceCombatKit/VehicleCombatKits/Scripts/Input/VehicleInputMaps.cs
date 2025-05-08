using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Vehicles;


namespace VSX.Controls
{
    /// <summary>
    /// Stores a list of input maps associated with a specific vehicle class.
    /// </summary>
    [System.Serializable]
    public class VehicleInputDisplaySettings
    {
        [Tooltip("The vehicle class the input maps are for.")]
        public VehicleClass vehicleClass;

        [Tooltip("The input maps for the vehicle.")]
        public List<string> maps = new List<string>();
    }
}
