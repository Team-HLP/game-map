using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Vehicles
{
    [CreateAssetMenu(menuName = "VSX/Module Type")]
    public class ModuleType : ScriptableObject 
    {
        [SerializeField]
        public List<string> labels = new List<string>();
        public List<string> Labels { get { return labels; } }
    }
}
