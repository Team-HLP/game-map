using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Creates an object and respawns a set time after it is deactivated.
    /// </summary>
    public class Respawner : MonoBehaviour
    {
        [SerializeField]
        protected float respawnTime = 5;

        [SerializeField]
        protected GameObject m_Object;
        protected GameObject createdGameObject;

        protected bool respawning = false;

        protected float respawnWaitStartTime = 0;


        private void Awake()
        {
            if (m_Object.scene.rootCount == 0)
            {
                createdGameObject = GameObject.Instantiate(m_Object, transform.position, Quaternion.identity);
            }
            else
            {
                createdGameObject = m_Object;
            }
            
            createdGameObject.transform.SetParent(transform);
        }

        protected virtual void Respawn()
        {
            createdGameObject.SetActive(true);
            respawning = false;
        }


        // Called every frame
        private void Update()
        {
            if (!createdGameObject.activeSelf)
            {
                // Wait for respawn
                if (!respawning)
                {
                    respawning = true;
                    respawnWaitStartTime = Time.time;
                }
                else
                {
                    // Respawn
                    if (Time.time - respawnWaitStartTime > respawnTime)
                    {
                        Respawn();
                    }
                }
            }
        }
    }
}