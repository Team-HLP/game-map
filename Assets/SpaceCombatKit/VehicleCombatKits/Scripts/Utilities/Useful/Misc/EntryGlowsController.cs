using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    public class EntryGlowsController : MonoBehaviour
    {
        public ParticleSystem ps;
        public float interval = 0.3f;
        public float spacing = 25;
        public int num = 8;
        float lastTime;

        int nextIndex = 0;

        private void Update()
        {
            if (Time.time - lastTime > interval)
            {
                lastTime = Mathf.FloorToInt(Time.time / interval) * interval;

                ps.transform.position = transform.position + transform.forward * spacing * (num - nextIndex);
                ps.Emit(1);

                nextIndex++;
                nextIndex %= num;

            }
        }
    }
}

