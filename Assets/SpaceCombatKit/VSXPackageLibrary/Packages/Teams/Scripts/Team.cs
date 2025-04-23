using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Teams
{
    /// <summary>
    /// Represents a team for target selection etc.
    /// </summary>
    [CreateAssetMenu(menuName = "VSX/Team")]
    public class Team : ScriptableObject
    {
        [SerializeField]
        protected Color defaultColor;
        public Color DefaultColor { get { return defaultColor; } }

        [SerializeField]
        protected List<Sprite> sprites = new List<Sprite>();
        public List<Sprite> Sprites { get { return sprites; } }

        [SerializeField]
        protected List<Team> hostileTeams = new List<Team>();
        public List<Team> HostileTeams
        {
            get { return hostileTeams; }
            set { hostileTeams = value; }
        }
    }
}
