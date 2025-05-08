using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.UI;
using UnityEngine.UI;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Unity event for running functions related to a timer.
    /// </summary>
    [System.Serializable]
    public class TimerEventHandler : UnityEvent { }

    [System.Serializable]
    public class OnTimerDisplayUpdatedEventHandler : UnityEvent<string> { }

    /// <summary>
    /// Timer component for running events when a timer is started and when it is finished.
    /// </summary>
    public class Timer : MonoBehaviour
    {
        [Header("General Settings")]

        [SerializeField]
        protected bool startTimerOnEnable = false;

        [SerializeField]
        protected float duration = 5;

        protected float nextDuration = 5;

        protected float startTime;

        protected bool running = false;
        public bool Running { get { return running; } }

        [SerializeField]
        protected float delay = 0f;
        protected float nextDelay;
        protected bool delayRunning = false;
        protected float delayStartTime;

        [SerializeField]
        protected List<TimerEvent> timerEvents = new List<TimerEvent>();

        [Header("Randomization")]

        [SerializeField]
        protected bool randomizedDuration = false;

        [SerializeField]
        protected float minRandomDuration = 1;

        [SerializeField]
        protected float maxRandomDuration = 5;

        [Header("Display")]

        [SerializeField]
        protected UVCText displayText;

        [SerializeField]
        protected Image fillBar;

        public enum DisplayType
        {
            Time,
            Percentage
        }

        [SerializeField]
        protected DisplayType displayType;

        [SerializeField]
        protected bool countDown = true;

        [SerializeField]
        protected string displayPrefix;

        [SerializeField]
        protected string displaySuffix;

        [Header("Events")]

        public TimerEventHandler onTimerStarted;

        public TimerEventHandler onTimerFinished;

        public OnTimerDisplayUpdatedEventHandler onTimerDisplayUpdated;

        [System.Serializable]
        public class TimerEvent
        {
            protected bool triggered;
            public bool Triggered
            {
                get { return triggered; }
                set { triggered = value; }
            }

            public float normalizedTime;
            public UnityEvent onTrigger;


            public virtual void Reset()
            {
                triggered = false;
            }

            public virtual void CheckTrigger(float normalizedTime)
            {
                if (!triggered)
                {
                    if (normalizedTime >= this.normalizedTime)
                    {
                        triggered = true;
                        onTrigger.Invoke();
                    }
                }
            }
        }

        

        protected virtual void OnEnable()
        {
            if (startTimerOnEnable)
            {
                StartTimer();
            }
        }

        /// <summary>
        /// Start the timer
        /// </summary>
        public virtual void StartTimer()
        {
            nextDuration = randomizedDuration ? Random.Range(minRandomDuration, maxRandomDuration) : duration;
            StartTimer(nextDuration);
        }

        public virtual void StartTimerDelayed(float delay)
        {
            nextDelay = delay;
            delayRunning = true;
            delayStartTime = Time.time;
        }

        /// <summary>
        /// Start the timer with a specific duration.
        /// </summary>
        /// <param name="newDuration">The new timer duration.</param>
        public virtual void StartTimer(float newDuration)
        {
            for (int i = 0; i < timerEvents.Count; ++i)
            {
                timerEvents[i].Reset();
            }

            nextDuration = newDuration;
            startTime = Time.time;
            running = true;
            onTimerStarted.Invoke();
        }


        public virtual void StopTimer()
        {
            running = false;
        }


        protected void DisplayUpdate(float normalizedTime)
        {
            string displayString = "";
            switch (displayType)
            {
                case DisplayType.Time:

                    float displayTime = (countDown ? (1 - normalizedTime) : normalizedTime) * nextDuration;

                    int minutes = (int)((displayTime) / 60);
                    int seconds = (int)((displayTime) - (minutes * 60));

                    string minutesString = minutes < 10 ? "0" + minutes.ToString() : minutes.ToString();
                    string secondsString = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();

                    displayString = minutesString + ":" + secondsString;

                    break;

                case DisplayType.Percentage:

                    float displayFraction = (countDown ? (nextDuration - normalizedTime) : normalizedTime);
                    displayString = (Mathf.RoundToInt(displayFraction * 100)).ToString();

                    break;
            }

            displayString = displayPrefix + displayString + displaySuffix;

            if (displayText != null)
            {
                displayText.text = displayString;
            }

            if (fillBar != null)
            {
                fillBar.fillAmount = (countDown ? (nextDuration - (Time.time - startTime)) : (Time.time - startTime)) / nextDuration;
            }

            onTimerDisplayUpdated.Invoke(displayString);

        }

        // Called every frame
        protected virtual void Update()
        {

            if (delayRunning)
            {
                if (Time.time - delayStartTime >= nextDelay)
                {
                    delayRunning = false;
                    StartTimer();
                }
            }

            // If it's running, check if the timer has finished.
            if (running)
            {
                if (Time.time - startTime >= nextDuration)
                {
                    DisplayUpdate(1);
                    running = false;
                    onTimerFinished.Invoke();
                }
                else
                {
                    DisplayUpdate((Time.time - startTime) / nextDuration);
                }

                for (int i = 0; i < timerEvents.Count; ++i)
                {
                    timerEvents[i].CheckTrigger((Time.time - startTime) / nextDuration);
                }
            }
        }
    }
}

