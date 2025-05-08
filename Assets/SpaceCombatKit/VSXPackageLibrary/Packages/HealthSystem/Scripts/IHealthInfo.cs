using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VSX.Health
{
    /// <summary>
    /// An interface for a component that contains health information.
    /// </summary>
    public interface IHealthInfo
    {
        float GetHealthCapacityByType(HealthType healthType);

        float GetCurrentHealthByType(HealthType healthType);

        float GetCurrentHealthFractionByType(HealthType healthType);

        UnityEvent OnHealthChanged { get; }
    }
}

