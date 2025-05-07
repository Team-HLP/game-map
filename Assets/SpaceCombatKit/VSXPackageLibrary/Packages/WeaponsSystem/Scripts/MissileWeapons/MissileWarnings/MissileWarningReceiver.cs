using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.Utilities.UI;

namespace VSX.Weapons
{
    public class MissileWarningReceiver : MonoBehaviour
    {
        public AudioSource warningAudio;
        public AudioSource toneAudio;

        protected float lastPlayTime;
        protected float audioClipLength;

        public float toneDistanceThreshold;
        public Vector2 minMaxBeepDistance = new Vector2(300, 1200);
        public Vector2 minMaxBeepInterval = new Vector2(0, 0.5f);

        public AnimationCurve distanceWarningIntervalCurve;

        public CanvasGroupFader textWarning;

        bool warningActive = false;
        protected float minDist = 0;

        public UnityEvent onWarningStarted;
        public UnityEvent onWarningStopped;
        public UnityEvent onMissileEvaded;


        private void Awake()
        {
            audioClipLength = warningAudio.clip.length;
        }


        public void OnMissileWarning(Missile missile)
        {
            if (!warningActive)
            {
                minDist = Vector3.Distance(transform.position, missile.transform.position);
                onWarningStarted.Invoke();
            }
            else
            {
                minDist = Mathf.Min(minDist, Vector3.Distance(transform.position, missile.transform.position));
            }
            warningActive = true;
        }

        public void OnLockLost()
        {
            if (warningActive)
            {
                StopWarning();
                onMissileEvaded.Invoke();
            }
        }


        public virtual void StopWarning()
        {
            if (toneAudio != null)
            {
                if (toneAudio.isPlaying)
                {
                    toneAudio.Stop();
                }
            }

            if (textWarning != null && textWarning.Animating)
            {
                textWarning.StopAnimation();
            }

            warningActive = false;

            onWarningStopped.Invoke();
        }


        private void LateUpdate()
        {
            if (warningActive)
            {
                if (minDist > toneDistanceThreshold)
                {
                    float val = distanceWarningIntervalCurve.Evaluate(Mathf.Clamp((minDist - minMaxBeepDistance.x) / (minMaxBeepDistance.y - minMaxBeepDistance.x), 0, 1));
                    float interval = val * minMaxBeepInterval.y + (1 - val) * minMaxBeepInterval.x;

                    if (warningAudio != null && Time.time - lastPlayTime >= interval + audioClipLength)
                    {
                        warningAudio.Play();
                        lastPlayTime = Time.time;
                    }
                }
                else
                {
                    if (toneAudio != null)
                    {
                        if (!toneAudio.isPlaying)
                        {
                            toneAudio.Play();
                        }
                    }
                }

                if (textWarning != null && !textWarning.Animating)
                {
                    textWarning.StartAnimation();
                }
            }
        }
    }
}

