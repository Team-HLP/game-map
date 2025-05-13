using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.Pooling;
using VSX.Vehicles;
using VSX.Utilities;
using VSX.Health;
using Tobii.XR;


namespace VSX.Weapons
{
    /// <summary>
    /// Unity event for running functions when a projectile is launched by a projectile launcher
    /// </summary>
    [System.Serializable]
    public class OnProjectileLauncherProjectileLaunchedEventHandler : UnityEvent<Projectile> { }

    /// <summary>
    /// This class spawns a projectile prefab at a specified interval and with a specified launch velocity.
    /// </summary>
    public class ProjectileWeaponUnit : WeaponUnit, IRootTransformUser, IGameAgentOwnable
    {
        protected GameAgent owner;
        public GameAgent Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        [Header("Settings")]

        [SerializeField]
        protected Transform spawnPoint;

        public override void Aim(Vector3 aimPosition)
        {
            if (aimAssistEnabled) spawnPoint.LookAt(aimPosition, transform.up);
        }
        public override void ClearAim() { spawnPoint.localRotation = Quaternion.identity; }

        [SerializeField]
        protected Projectile projectilePrefab;
        public Projectile ProjectilePrefab
        {
            get { return projectilePrefab; }
            set { projectilePrefab = value; }
        }

        [SerializeField]
        protected bool usePoolManager;

        [SerializeField]
        protected bool addLauncherVelocityToProjectile;

        [Tooltip("Additional velocity to add to the projectile (relative to the launcher). Can be used e.g. to make a missile move out to the side to prevent exhaust blocking the view.")]
        [SerializeField]
        protected Vector3 projectileRelativeImpulseVelocity = Vector3.zero;

        [SerializeField]
        protected float maxInaccuracyAngle = 2;
        public float MaxInaccuracyAngle
        {
            get { return maxInaccuracyAngle; }
            set { maxInaccuracyAngle = value; }
        }

        [Range(0, 1)]
        [SerializeField]
        protected float accuracy = 1;
        public float Accuracy
        {
            get { return accuracy; }
            set { accuracy = value; }
        }

        [Header("Events")]

        // Projectile launched event
        public OnProjectileLauncherProjectileLaunchedEventHandler onProjectileLaunched;

        protected float damageMultiplier;
        protected float healingMultiplier;

        protected Transform rootTransform;
        public Transform RootTransform
        {
            set
            {
                rootTransform = value;

                if (rootTransform != null)
                {
                    rBody = rootTransform.GetComponent<Rigidbody>();
                }
                else
                {
                    rBody = null;
                }
            }
        }

        protected Rigidbody rBody;

        public override float Speed
        {
            get { return projectilePrefab != null ? projectilePrefab.Speed : 0; }
        }

        public override float Range
        {
            get { return projectilePrefab != null ? projectilePrefab.Range : 0; }
        }

        public override float Damage(HealthType healthType)
        {
            if (projectilePrefab != null)
            {
                return projectilePrefab.Damage(healthType);
            }
            else
            {
                return 0;
            }
        }

        public override float Healing(HealthType healthType)
        {
            if (projectilePrefab != null)
            {
                return projectilePrefab.Healing(healthType);
            }
            else
            {
                return 0;
            }
        }


        protected override void Reset()
        {
            base.Reset();

            spawnPoint = transform;

            Projectile defaultProjectilePrefab = Resources.Load<Projectile>("SCK/Projectile");
            if (defaultProjectilePrefab != null)
            {
                projectilePrefab = defaultProjectilePrefab;
            }
        }


        protected virtual void Awake()
        {

            if (rootTransform == null) rootTransform = transform.root;

            if (rootTransform != null)
            {
                rBody = rootTransform.GetComponent<Rigidbody>();
            }
        }


        [SerializeField] private float gazeFireInterval = 0.2f; // 자동 발사 간격
        private float nextFireTime = 0f;
        private bool isGazingAtTarget = false;

        // GazeTarget 레이어만 감지하기 위한 LayerMask
        private int gazeLayerMask;
        private void Start()
        {
            if (usePoolManager && PoolManager.Instance == null)
            {
                usePoolManager = false;
                Debug.LogWarning("No PoolManager component found in scene, please add one to pool projectiles.");
            }

            gazeLayerMask = LayerMask.GetMask("GazeTarget");  // 레이어 마스크 설정
        }

        private void Update()
        {
            var eyeData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);

            if (!eyeData.GazeRay.IsValid) return;

            Ray gazeRay = new Ray(eyeData.GazeRay.Origin, eyeData.GazeRay.Direction);

            if (Physics.Raycast(gazeRay, out RaycastHit hit, 1000f, gazeLayerMask))
            {
                // 응시 대상 감지 성공
                if (Time.time >= nextFireTime)
                {
                    spawnPoint.rotation = Quaternion.LookRotation(gazeRay.direction);

                    // 🔥 레이저 발사
                    TriggerOnce();

                    // ✅ 다음 발사 시간 정확히 갱신
                    nextFireTime = Time.time + gazeFireInterval;
                }
            }
        }


        /// <summary>
        /// Set the damage multiplier for this weapon unit.
        /// </summary>
        /// <param name="damageMultiplier">The damage multiplier.</param>
        public override void SetDamageMultiplier(float damageMultiplier)
        {
            this.damageMultiplier = damageMultiplier;
        }


        /// <summary>
        /// Set the healing multiplier for this weapon unit.
        /// </summary>
        /// <param name="healingMultiplier">The healing multiplier.</param>
        public override void SetHealingMultiplier(float healingMultiplier)
        {
            this.healingMultiplier = healingMultiplier;
        }


        // Launch a projectile
        public override void TriggerOnce()
        {
            if (projectilePrefab != null)
            {
                float nextMaxInaccuracyAngle = maxInaccuracyAngle * (1 - accuracy);

                // 👇 기존 회전 흔들림 삭제 (시선 기반이면 불필요하거나 보완 가능)
                // spawnPoint.Rotate(...)

                // 👉 ① 시선 정보 가져오기
                var eyeData = Tobii.XR.TobiiXR.GetEyeTrackingData(Tobii.XR.TobiiXR_TrackingSpace.World);
                if (eyeData.GazeRay.IsValid)
                {
                    // 👉 ② 시선 방향으로 회전 설정
                    spawnPoint.rotation = Quaternion.LookRotation(eyeData.GazeRay.Direction);
                }

                // 👉 ③ 발사체 생성 (위치 그대로, 회전은 시선)
                Projectile projectileController;
                if (usePoolManager)
                {
                    projectileController = PoolManager.Instance.Get(projectilePrefab.gameObject, spawnPoint.position, spawnPoint.rotation).GetComponent<Projectile>();
                }
                else
                {
                    projectileController = GameObject.Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
                }

                // 👇 나머지 기존 유지
                projectileController.SetOwner(owner);
                projectileController.SetSenderRootTransform(rootTransform);

                if (addLauncherVelocityToProjectile && rBody != null)
                {
                    projectileController.AddVelocity(rBody.velocity);
                    projectileController.AddVelocity(transform.TransformDirection(projectileRelativeImpulseVelocity));
                }

                onProjectileLaunched.Invoke(projectileController);
            }

            ClearAim();
        }

    }
}