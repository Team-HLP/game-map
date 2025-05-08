using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    /// <summary>
    /// Interface for something that aims (for example a weapon).
    /// </summary>
    public interface IAimer
    {
        void Aim(Vector3 aimPosition);

        Vector3 AimPosition();

        Vector3 ZeroAimDirection();

        void ClearAim();

        void SetAimingEnabled(bool aimingEnabled);
    }
}
