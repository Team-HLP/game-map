using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    /// <summary>
    /// Interface for a component that stores lead target position information
    /// </summary>
    public interface ILeadTargetInfo
    {
        Transform Target { get; }

        List<Vector3> LeadTargetPositions { get; }

        int LeadTargetAimedIndex { get; }

        float AimStateStartTime { get; }
    }
}

