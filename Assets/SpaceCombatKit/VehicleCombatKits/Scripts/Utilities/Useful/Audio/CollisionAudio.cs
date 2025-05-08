using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Effects
{
    /// <summary>
    /// Play audio based on collision velocity.
    /// </summary>
    public class CollisionAudio : MonoBehaviour
    {

        [Tooltip("The audio source for playing collision sounds.")]
        [SerializeField]
        protected AudioSource m_AudioSource;

        [Tooltip("The audio clips to choose a random sound from.")]
        [SerializeField]
        protected List<AudioClip> clips = new List<AudioClip>();

        [Tooltip("The curve describing the audio volume (Y-axis) relative to collision speed (X-axis).")]
        [SerializeField]
        protected AnimationCurve collisionVelocityToVolumeCurve = AnimationCurve.Linear(0, 0, 100, 1);

        [Tooltip("The minimum random pitch for the collision sound.")]
        [SerializeField]
        protected float minRandomPitch = 1;

        [Tooltip("The maximum random pitch for the collision sound.")]
        [SerializeField]
        protected float maxRandomPitch = 1;


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
            if (clips.Count != 0 && m_AudioSource != null && !m_AudioSource.isPlaying)
            {
                m_AudioSource.volume = collisionVelocityToVolumeCurve.Evaluate(collision.relativeVelocity.magnitude);
                m_AudioSource.pitch = Random.Range(minRandomPitch, maxRandomPitch);
                m_AudioSource.clip = clips[Random.Range(0, clips.Count)];
                m_AudioSource.Play();
            }
        }
    }
}

