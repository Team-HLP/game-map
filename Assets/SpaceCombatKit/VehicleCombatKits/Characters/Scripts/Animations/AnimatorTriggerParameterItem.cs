﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Characters
{
    /// <summary>
    /// Set up an animator trigger parameter that stores the parameter hash so it can be efficiently updated.
    /// </summary>
    [System.Serializable]
    public class AnimatorTriggerParameterItem
    {
        [Tooltip("The animator to set the parameter on.")]
        public Animator animator;

        [Tooltip("The parameter name.")]
        public string triggerParameterName;

        protected int triggerParameterHash;

        /// <summary>
        /// Initialize the hash.
        /// </summary>
        public virtual void Initialize()
        {
            triggerParameterHash = Animator.StringToHash(triggerParameterName);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="animator">The animator to set the parameter on.</param>
        /// <param name="triggerParameterName">The parameter name.</param>
        public AnimatorTriggerParameterItem(Animator animator, string triggerParameterName)
        {
            this.animator = animator;
            this.triggerParameterName = triggerParameterName;
            
            triggerParameterHash = Animator.StringToHash(triggerParameterName);
        }

        /// <summary>
        /// Set the parameter on the animator.
        /// </summary>
        /// <param name="value">The parameter value.</param>
        public virtual void SetParameter()
        {
            animator.SetTrigger(triggerParameterHash);
        }
    }
}
