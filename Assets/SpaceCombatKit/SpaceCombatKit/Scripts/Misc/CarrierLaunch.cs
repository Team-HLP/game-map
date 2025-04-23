using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.Rumbles;
using VSX.Loadouts;
using VSX.Vehicles;
using VSX.Engines3D;
using VSX.GameStates;


namespace VSX.SpaceCombatKit
{
    public class CarrierLaunch : MonoBehaviour
    {

        [SerializeField]
        protected Vehicle startingLaunchVehicle;

        public LoadoutVehicleSpawner spawner;

        [SerializeField]
        protected bool launchOnStart;

        [SerializeField]
        protected List<Collider> carrierColliders = new List<Collider>();


        [Header("Launch Parameters")]

        [SerializeField]
        protected Transform launchStart;

        [SerializeField]
        protected Transform launchEnd;

        [SerializeField]
        protected float launchTime = 3;

        [SerializeField]
        protected float launchDelay = 2;


        [Header("Animation Curves")]

        [SerializeField]
        protected AnimationCurve launchCurve;

        [SerializeField]
        protected AnimationCurve throttleCurve;

        [SerializeField]
        protected AnimationCurve rumbleCurve;


        [Header("Game States")]

        [SerializeField]
        protected GameState launchingGameState;

        [SerializeField]
        protected GameState launchingFinishedGameState;

        public AudioSource launchAudio;
        public float launchAudioDelay = 2;


        [Header("Events")]

        public UnityEvent onLaunched;

        protected float lastPositionTime;


        private void Awake()
        {
            // Disable the carrier colliders at the start
            for (int i = 0; i < carrierColliders.Count; ++i)
            {
                carrierColliders[i].enabled = false;
            }

            if (spawner != null) spawner.onSpawned.AddListener(OnSpawnerSpawned);
        }

        private void Start()
        {
            if (launchOnStart && startingLaunchVehicle != null)
            {
                Launch(startingLaunchVehicle);
            }
        }

        void OnSpawnerSpawned()
        {
            Launch(GameAgentManager.Instance.FocusedGameAgent.Vehicle);
        }

        /// <summary>
        /// Launch a vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle to launch.</param>
        public void Launch(Vehicle vehicle)
        {
            StartCoroutine(LaunchCoroutine(vehicle));
        }


        // Launch coroutine
        protected IEnumerator LaunchCoroutine(Vehicle vehicle)
        {

            GameStateManager.Instance.EnterGameState(launchingGameState);
            VehicleEngines3D engines = vehicle.GetComponent<VehicleEngines3D>();

            // Prepare the vehicle
            vehicle.transform.position = launchStart.position;
            vehicle.transform.LookAt(launchEnd, Vector3.up);
            engines.SetMovementInputs(new Vector3(0, 0, 0));
            vehicle.CachedRigidbody.isKinematic = true;


            yield return new WaitForSeconds(launchDelay);


            // Prepare data
            Vector3 _velocity = Vector3.zero;
            Vector3 lastPosition = Vector3.zero;
            float startTime = Time.time;

            if (launchAudio != null) launchAudio.PlayDelayed(launchAudioDelay);

            // Animate
            while (true)
            {

                float timeAmount = (Time.time - startTime) / launchTime;

                

                // Check if finished animation
                if (timeAmount > 1)
                {
                    // Enter new game state
                    GameStateManager.Instance.EnterGameState(launchingFinishedGameState);

                    // Prepare vehicle for gameplay
                    vehicle.CachedRigidbody.isKinematic = false;
                    vehicle.CachedRigidbody.velocity = _velocity;
                    
                    engines.SetMovementInputs(new Vector3(0, 0, 1));

                    // Enable carrier colliders
                    for (int i = 0; i < carrierColliders.Count; ++i)
                    {
                        carrierColliders[i].enabled = true;
                    }

                    // Call the launched event
                    onLaunched.Invoke();

                    break;
                }
                else
                {
                    // Move the vehicle
                    float launchAmount = launchCurve.Evaluate(timeAmount);
                    vehicle.CachedRigidbody.MovePosition(launchAmount * launchEnd.position + (1 - launchAmount) * launchStart.position);

                    // Update the throttle
                    engines.SetMovementInputs(new Vector3(0, 0, throttleCurve.Evaluate(timeAmount)));

                    // Add a rumble
                    RumbleManager.Instance.AddSingleFrameRumble(rumbleCurve.Evaluate(timeAmount), vehicle.transform.position);

                    if (!Mathf.Approximately(Time.time - lastPositionTime, 0))
                    {
                        _velocity = (vehicle.CachedRigidbody.position - lastPosition) / (Time.time - lastPositionTime);
                        lastPosition = vehicle.CachedRigidbody.position;
                        lastPositionTime = Time.time;
                    }
                }

                yield return new WaitForFixedUpdate();
            }
        }
    }
}


