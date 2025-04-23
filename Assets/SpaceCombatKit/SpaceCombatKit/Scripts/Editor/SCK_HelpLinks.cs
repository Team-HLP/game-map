using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Adds links to help resources in the editor top menu.
    /// </summary>
    public class SCK_HelpLinks : EditorWindow
    {
        [MenuItem("Tools/Space Combat Kit/Help/Tutorial Videos")]
        public static void TutorialVideos()
        {
            Application.OpenURL("https://vimeo.com/showcase/6603196");
        }

        [MenuItem("Tools/Space Combat Kit/Help/Documentation")]
        public static void Documentation()
        {
            Application.OpenURL("https://vsxgames.gitbook.io/universal-vehicle-combat/");
        }

        [MenuItem("Tools/Space Combat Kit/Help/Forum")]
        public static void Forum()
        {
            Application.OpenURL("https://forum.unity.com/threads/space-combat-kit-vsxgames-released.340962/");
        }
    }
}

