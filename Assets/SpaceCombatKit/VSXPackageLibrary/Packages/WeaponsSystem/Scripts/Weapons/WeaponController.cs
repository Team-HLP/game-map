using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.RadarSystem;

namespace VSX.Weapons
{

    public class WeaponController : MonoBehaviour
    {
        public Weapon weapon;

        public virtual void SetTarget(Trackable target) { }
    }
}
