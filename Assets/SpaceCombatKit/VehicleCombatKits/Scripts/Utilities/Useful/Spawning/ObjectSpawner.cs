using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VSX.Pooling;

namespace VSX.Utilities
{
    /// <summary>
    /// Instantiate one or more objects in the scene.
    /// </summary>
    public class ObjectSpawner : MonoBehaviour
    {

        [Tooltip("Whether to spawn the objects when this object is enabled.")]
        [SerializeField]
        protected bool spawnOnEnable = true;

        [Tooltip("Whether to use object pooling or instantiate a new instance every time.")]
        [SerializeField]
        protected bool usePoolManager = true;

        [Tooltip("A list of gameobjects to be spawned.")]
        [SerializeField]
        protected List<GameObject> objectsToSpawn = new List<GameObject>();

        [Tooltip("The transform representing the position and rotation to spawn objects with.")]
        [SerializeField]
        protected Transform spawnTransform;

        [Tooltip("Whether to parent the object to the spawn transform.")]
        [SerializeField]
        protected bool parentToSpawnTransform = false;

        [Tooltip("The scale that will be applied to the objects on spawn.")]
        [SerializeField]
        protected float scale = 1;

        [SerializeField]
        protected bool addTransformScaleToObject = true;

        protected bool started = false;


        protected virtual void Reset()
        {
            spawnTransform = transform;
        }


        protected virtual void OnEnable()
        {
            if (spawnOnEnable && started)
            {
                SpawnAll();
            }
        }


        protected virtual void Start()
        {
            started = true;

            if (spawnOnEnable) SpawnAll(); // Prevents spawning when object is immediately deactivated (e.g. pooling)
        }


        // Spawn all


        /// <summary>
        /// Spawn all the objects in the list.
        /// </summary>
        public virtual void SpawnAll()
        {
            SpawnAll(spawnTransform.position, spawnTransform.rotation);
        }


        /// <summary>
        /// Spawn all the objects in the list at a specified position.
        /// </summary>
        /// <param name="position">The spawn position.</param>
        public virtual void SpawnAll(Vector3 position)
        {
            SpawnAll(position, spawnTransform.rotation);
        }


        /// <summary>
        /// Spawn all the objects in the list at a specified rotation.
        /// </summary>
        /// <param name="rotation">The spawn rotation.</param>
        public virtual void SpawnAll(Quaternion rotation)
        {
            SpawnAll(spawnTransform.position, rotation);
        }


        /// <summary>
        /// Spawn all the objects in the list at a specified position and rotation.
        /// </summary>
        /// <param name="position">The spawn position.</param>
        /// <param name="rotation">The spawn rotation</param>
        public virtual void SpawnAll(Vector3 position, Quaternion rotation)
        {
            for (int i = 0; i < objectsToSpawn.Count; ++i)
            {
                SpawnObject(objectsToSpawn[i], position, rotation);
            }
        }


        // By index


        /// <summary>
        /// Spawn an object at a specified index in the list.
        /// </summary>
        /// <param name="index">The list index.</param>
        public virtual void SpawnByIndex(int index)
        {
            if (objectsToSpawn.Count >= index)
            {
                SpawnObject(objectsToSpawn[index], spawnTransform.position, spawnTransform.rotation);
            }
        }


        /// <summary>
        /// Spawn an object at a specified index in the list, at a specified position.
        /// </summary>
        /// <param name="index">The list index.</param>
        /// <param name="position">The spawn position.</param>
        public virtual void SpawnByIndex(int index, Vector3 position) 
        {
            SpawnObject(objectsToSpawn[index], position, spawnTransform.rotation);
        }


        /// <summary>
        /// Spawn an object at a specified index in the list, at a specified rotation.
        /// </summary>
        /// <param name="index">The list index.</param>
        /// <param name="rotation">The spawn rotation.</param>
        public virtual void SpawnByIndex(int index, Quaternion rotation) 
        {
            SpawnObject(objectsToSpawn[index], spawnTransform.position, rotation);
        }


        /// <summary>
        /// Spawn an object at a specified index in the list, at a specified position and rotation.
        /// </summary>
        /// <param name="index">The list index.</param>
        /// <param name="position">The spawn position.</param>
        /// <param name="rotation">The spawn rotation.</param>
        public virtual void SpawnByIndex(int index, Vector3 position, Quaternion rotation) 
        {
            SpawnObject(objectsToSpawn[index], position, rotation);
        }


        // Random


        /// <summary>
        /// Spawn a random object from the list.
        /// </summary>
        public virtual void SpawnRandom()
        {
            SpawnByIndex(Random.Range(0, objectsToSpawn.Count), spawnTransform.position, spawnTransform.rotation);
        }


        /// <summary>
        /// Spawn a random object from the list at a specified position.
        /// </summary>
        /// <param name="position">The spawn position.</param>
        public virtual void SpawnRandom(Vector3 position)
        {
            SpawnByIndex(Random.Range(0, objectsToSpawn.Count), position, spawnTransform.rotation);
        }


        /// <summary>
        /// Spawn a random object from the list at a specified rotation.
        /// </summary>
        /// <param name="rotation">The spawn rotation.</param>
        public virtual void SpawnRandom(Quaternion rotation)
        {
            SpawnByIndex(Random.Range(0, objectsToSpawn.Count), spawnTransform.position, rotation);
        }


        /// <summary>
        /// Spawn a random object from the list at a specified position and rotation.
        /// </summary>
        /// <param name="position">The spawn position.</param>
        /// <param name="rotation">The spawn rotation.</param>
        public virtual void SpawnRandom(Vector3 position, Quaternion rotation)
        {
            SpawnByIndex(Random.Range(0, objectsToSpawn.Count), position, rotation);
        }


        /// <summary>
        /// Spawn an object.
        /// </summary>
        /// <param name="objectToSpawn"></param>
        protected virtual GameObject SpawnObject(GameObject objectToSpawn, Vector3 position, Quaternion rotation)
        {
            GameObject obj;
            if (usePoolManager && PoolManager.Instance != null)
            {
                obj = PoolManager.Instance.Get(objectToSpawn, position, rotation, parentToSpawnTransform ? spawnTransform : null);
            }
            else
            {
                obj = Instantiate(objectToSpawn, spawnTransform.position, spawnTransform.rotation);
                if (parentToSpawnTransform)
                {
                    obj.transform.SetParent(spawnTransform);
                }
            }

            float nextScale = scale * (addTransformScaleToObject ? (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 3f : 1);

            ObjectScaleController scaleController = obj.GetComponent<ObjectScaleController>();
            if (scaleController != null)
            {
                scaleController.SetScale(nextScale);
            }
            else
            {
                obj.transform.localScale = new Vector3(nextScale, nextScale, nextScale);
            }

            return obj;
        }
    }
}

