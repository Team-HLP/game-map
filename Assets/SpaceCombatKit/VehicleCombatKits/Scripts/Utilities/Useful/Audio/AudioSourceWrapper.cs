using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    /// <summary>
    /// Wrapper around an audio source. Enables checking if the audio source is active, preventing unnecessary warnings when trying to play disabled audio.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceWrapper : MonoBehaviour
    {
        protected AudioSource audioSource;


        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }


        /// <summary>
        /// Play the audio source.
        /// </summary>
        public virtual void Play()
        {
            if (!gameObject.activeInHierarchy) return;

            audioSource.Play();
        }
    }
}

