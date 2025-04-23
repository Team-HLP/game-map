using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Rumbles
{
    /// <summary>
    /// Play a rumble when a collision occurs.
    /// </summary>
    public class CollisionRumble : MonoBehaviour
    {

        [Tooltip("The rumble animation to play when collision occurs.")]
        [SerializeField]
        protected AddRumble collisionRumble;

        [Tooltip("The curve describing the rumble level (Y-axis) relative to the collision speed (X-axis).")]
        [SerializeField]
        protected AnimationCurve collisionVelocityToRumbleLevel = AnimationCurve.Linear(0, 0, 20, 0.2f);


        // Called by physics engine when collision occurs.
        protected virtual void OnCollisionEnter(Collision collision)
        {
            OnCollision(collision);
        }


        /// <summary>
        /// Called when a collision occurs.
        /// </summary>
        /// <param name="collision">The collision data.</param>
        public virtual void OnCollision(Collision collision)
        {
            if (collisionRumble != null)
            {
                collisionRumble.MaxLevel = collisionVelocityToRumbleLevel.Evaluate(collision.relativeVelocity.magnitude);
                collisionRumble.Run();
            }
        }
    }

}
