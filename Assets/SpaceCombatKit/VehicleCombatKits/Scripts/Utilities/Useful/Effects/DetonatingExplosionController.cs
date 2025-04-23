using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Pooling;

namespace VSX.UniversalVehicleCombat
{
    public class DetonatingExplosionController : MonoBehaviour
    {
        public ParticleSystem baseParticleSystem;
        public Vector2 minMaxInterval = new Vector2(0.1f, 1f);
        public GameObject audioPrefab;


        private void OnEnable()
        {
            StartCoroutine(ExplosionSounds());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        IEnumerator ExplosionSounds()
        {
            while (true)
            {
                baseParticleSystem.Emit(1);

                if (PoolManager.Instance != null)
                {
                    PoolManager.Instance.Get(audioPrefab, transform.position, transform.rotation);
                }
                else
                {
                    Instantiate(audioPrefab, transform.position, transform.rotation);
                }
                
                yield return new WaitForSeconds(Random.Range(minMaxInterval.x, minMaxInterval.y));
            }
        }
    }
}

