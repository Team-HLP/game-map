using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VSX.Vehicles
{
    /// <summary>
    /// Designates an object which can be owned by a game agent (player or AI). Useful for passing ownership information to modules, projectiles, etc.
    /// </summary>
    public interface IGameAgentOwnable
    {
        GameAgent Owner { get; set; }
    }
}
