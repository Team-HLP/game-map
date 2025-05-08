using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Teams
{
    /// <summary>
    /// Associates a team with a color for showing team colors on the HUD.
    /// </summary>
    [System.Serializable]
    public class TeamColor
    {

        public Team team;

        public Color color = Color.white;

    }
}
