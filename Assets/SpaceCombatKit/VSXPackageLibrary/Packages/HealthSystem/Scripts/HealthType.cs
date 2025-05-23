﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Health
{
    /// <summary>
    /// Represents a health type for damage/healing.
    /// </summary>
    [CreateAssetMenu(menuName = "VSX/Health Type")]
    public class HealthType : ScriptableObject 
    {
        [SerializeField]
        protected Color m_Color = Color.white;
        public Color Color
        {
            get { return m_Color; }
        }
    }
}

