using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using VSX.Teams;
using VSX.Utilities;

namespace VSX.RadarSystem
{
    /// <summary>
    /// Select a target from a list of trackables.
    /// </summary>
    public class TargetSelector : MonoBehaviour
    {
        /// The list of trackables this selector is working with.
        protected List<Trackable> trackables = new List<Trackable>();


        [Header("Selection Criteria")]

        [Tooltip("The teams that can be selected. Leave empty to select all teams.")]
        [SerializeField]
        protected List<Team> selectableTeams = new List<Team>();
        public List<Team> SelectableTeams
        {
            get { return selectableTeams; }
            set { selectableTeams = value; }
        }


        [Tooltip("The types of trackable that can be selected. Leave empty to select all types.")]
        [SerializeField]
        protected List<TrackableType> selectableTypes = new List<TrackableType>();
        public List<TrackableType> SelectableTypes
        {
            get { return selectableTypes; }
            set { selectableTypes = value; }
        }


        [Tooltip("By default, targets at this depth are available for selection. Set to -1 for all depths.")]
        [SerializeField]
        protected int defaultDepth = -1;


        [Tooltip("Whether to always select the highest depth child of Trackable if it has child Trackables. E.g. if you want to automatically select the first available subsystem when a vehicle is selected.")]
        [SerializeField]
        protected bool selectHighestDepthChild = false;



        [Header("General")]

        [Tooltip("Whether to switch to a new target when it is aimed at.")]
        [SerializeField]
        protected bool aimTargetSelection = true;


        [Tooltip("The aim controller to use for aim-based target selection. Don't set this if you don't want to automatically switch to whatever the Aim Controller is aiming at.")]
        [SerializeField]
        protected AimControllerBase aimController;


        [Tooltip("The maximum aim angle within which a new target can be selected.")]
        [SerializeField]
        protected float aimTargetSelectionAngle = 3;


        [Tooltip("Whether to automatically and continuously scan for a new target when none are selected.")]
        [SerializeField]
        protected bool scanEveryFrame = true;
        public virtual bool ScanEveryFrame
        {
            get => scanEveryFrame;
            set => scanEveryFrame = value;
        }

        public enum TargetSelectionMode
        {
            Next,
            Previous,
            Nearest,
            Front,
            Random
        }


        [Tooltip("The default target selection mode when scanning for a new target.")]
        [SerializeField]
        protected TargetSelectionMode defaultTargetSelectionMode;


        [Tooltip("The angle within which to look for targets in front when the Select Front function is called.")]
        [SerializeField]
        protected float frontTargetAngle = 10;


        [Tooltip("The transform whose position and direction represents the vehicle when looking for front targets.")]
        [SerializeField]
        protected Transform frontTargetReference;


        [Tooltip("Whether to call the select event on a target.")]
        [SerializeField]
        protected bool callSelectEventOnTarget = true;


        protected Trackable selectedTarget;
        public Trackable SelectedTarget
        {
            get { return selectedTarget; }
            set { selectedTarget = value; }
        }



        [Header("Audio")]


        [Tooltip("Whether audio for target selection is enabled.")]
        [SerializeField]
        protected bool audioEnabled = true;
        public bool AudioEnabled
        {
            get { return audioEnabled; }
            set { audioEnabled = value; }
        }


        [Tooltip("The audio played when the selected target is changed.")]
        [SerializeField]
        protected AudioSource selectedTargetChangedAudio;



        [Header("Events")]

        [Tooltip("Event called when the selected target changes.")]
        public TrackableEventHandler onSelectedTargetChanged;




        // Called when the component is first added to a gameobject, or the component is reset in the inspector
        protected virtual void Reset()
        {
            frontTargetReference = transform;
        }


        // Get the index of the currently selected target in the list
        protected virtual int GetSelectedTargetIndex()
        {

            if (selectedTarget == null) return -1;

            for (int i = 0; i < trackables.Count; ++i)
            {
                if (trackables[i] == selectedTarget)
                {
                    return i;
                }
            }

            return -1;
        }


        /// <summary>
        /// Select the first selectable target.
        /// </summary>
        public virtual void SelectFirstSelectableTarget(int depth = -1)
        {
            for (int i = 0; i < trackables.Count; ++i)
            {
                if (depth != -1 && trackables[i].Depth != depth) continue;

                if (IsSelectable(trackables[i]))
                {
                    SetSelected(trackables[i]);
                    return;
                }
            }

            if (selectedTarget != null) SetSelected(null);
        }


        /// <summary>
        /// Get the front most target. This function does not take into account the front target angle limits.
        /// </summary>
        /// <param name="position">The reference position.</param>
        /// <param name="forwardDirection">The forward direction of the vehicle.</param>
        /// <param name="depth">What depth of child trackable (subsystem) to search for. Leave at -1 for no child target (subsystem) selection.</param>
        /// <returns>The front most target.</returns>
        public virtual Trackable GetFrontMostTarget(Vector3 position, Vector3 forwardDirection, int depth = -1)
        {
            float minAngle = 180;

            // Get the target that is nearest the forward vector of the tracker
            int index = -1;
            for (int i = 0; i < trackables.Count; ++i)
            {
                if (depth != -1 && trackables[i].Depth != depth) continue;

                if (IsSelectable(trackables[i]))
                {
                    float angle = Vector3.Angle(trackables[i].transform.position - position, forwardDirection);

                    if (angle < minAngle)
                    {
                        minAngle = angle;
                        index = i;
                    }
                }
            }

            // Select the target
            if (index != -1)
            {
                return (trackables[index]);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Check if a target is selectable by this target selector.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>Whether the target is selectable.</returns>
        public virtual bool IsSelectable(Trackable target)
        {

            // Check if the team is selectable
            if (selectableTeams.Count > 0)
            {
                bool teamFound = false;
                for (int i = 0; i < selectableTeams.Count; ++i)
                {
                    if (selectableTeams[i] == target.Team)
                    {
                        teamFound = true;
                        break;
                    }
                }
                if (!teamFound) return false;
            }

            // Check if the type is selectable 
            if (selectableTypes.Count > 0)
            {
                bool typeFound = false;
                for (int i = 0; i < selectableTypes.Count; ++i)
                {
                    if (selectableTypes[i] == target.TrackableType)
                    {
                        typeFound = true;
                        break;
                    }
                }
                if (!typeFound) return false;
            }

            return true;

        }


        /// <summary>
        /// Called when the Tracker stops tracking a target.
        /// </summary>
        /// <param name="trackable">The target that's stopped being tracked by the Tracker.</param>
        public virtual void OnStoppedTracking(Trackable trackable)
        {
            if (trackable == selectedTarget)
            {
                SetSelected(null);
            }
        }


        /// <summary>
        /// Select a specified target.
        /// </summary>
        /// <param name="target">The target to select.</param>
        public virtual void Select(Trackable target)
        {
            if (target == selectedTarget) return;

            if (target != null && !IsSelectable(target)) return;

            SetSelected(target);
        }


        protected virtual void SetSelected(Trackable newSelectedTarget)
        {
            if (newSelectedTarget == selectedTarget) return;

            // Unselect the currently selected target
            if (selectedTarget != null)
            {
                selectedTarget.Unselect();
            }

            if (newSelectedTarget != null)
            {
                // If toggled, select the highest depth child in the hierarchy.
                if (selectHighestDepthChild)
                {
                    for (int i = 0; i < 1000; ++i)
                    {
                        if (newSelectedTarget.ChildTrackables.Count > 0)
                        {
                            SetSelected(newSelectedTarget.ChildTrackables[0]);
                            return;
                        }
                    }
                }
            }

            // Play audio
            if (audioEnabled && selectedTargetChangedAudio != null)
            {
                // If new target is not null and is different from previous, play audio
                if (newSelectedTarget != null && newSelectedTarget != selectedTarget)
                {
                    selectedTargetChangedAudio.Play();
                }
            }

            // Update the target 
            selectedTarget = newSelectedTarget;

            // Call select event on the new target
            if (selectedTarget != null && callSelectEventOnTarget)
            {
                selectedTarget.Select();
            }

            // Call the event
            onSelectedTargetChanged.Invoke(selectedTarget);

        }


        /// <summary>
        /// Cycle back or forward through the targets list.
        /// </summary>
        /// <param name="forward">Whether to cycle forward.</param>
        public virtual void Cycle(bool forward, bool searchLocal = true, int depth = -1)
        {

            // Get the index of the currently selected target
            int index = GetSelectedTargetIndex();

            // If the selected target is null or doesn't exist in the list, just get the first selectable target
            if (index == -1)
            {
                SelectFirstSelectableTarget(depth);
                return;
            }

            List<Trackable> targetsList = trackables;
            Trackable rootTrackable = selectedTarget;
            if (searchLocal && depth > 0)
            {
                while (true)
                {
                    if (rootTrackable.ChildTrackables.Count == 0) return;

                    if (rootTrackable.ChildTrackables[0].Depth == depth)
                    {
                        break;
                    }
                    else
                    {
                        rootTrackable = rootTrackable.ChildTrackables[0];
                    }
                }

                targetsList = rootTrackable.ChildTrackables;
            }


            // Step through the targets in the specified direction looking for the next selectable one
            int direction = forward ? 1 : -1;
            for (int i = 0; i < targetsList.Count; ++i)
            {

                index += direction;

                // Wrap at the end
                if (index >= targetsList.Count)
                {
                    index = 0;
                }

                // Wrap at the beginning
                else if (index < 0)
                {
                    index = targetsList.Count - 1;
                }

                if (depth != -1 && targetsList[index].Depth != depth) continue;

                // Select the target if it's selectable
                if (IsSelectable(targetsList[index]))
                {
                    SetSelected(targetsList[index]);
                    return;
                }
            }

            if (selectedTarget != null) SetSelected(null);

        }


        /// <summary>
        /// Select the nearest target.
        /// </summary>
        /// <param name="depth">The depth of child (subsystem) to search for. Leave at -1 for no subsystem selection.</param>
        public virtual void SelectNearest(int depth = -1)
        {
            // Find the index of the target that is nearest
            float minDist = float.MaxValue;
            int index = -1;
            for (int i = 0; i < trackables.Count; ++i)
            {
                if (depth != -1 && trackables[i].Depth != depth) continue;

                if (IsSelectable(trackables[i]))
                {
                    float dist = Vector3.Distance(trackables[i].transform.position, transform.position);

                    if (dist < minDist)
                    {
                        minDist = dist;
                        index = i;
                    }
                }
            }

            // Select the target
            if (index != -1)
            {
                SetSelected(trackables[index]);
            }
            else
            {
                if (selectedTarget != null) SetSelected(null);
            }
        }


        /// <summary>
        /// Select the front most target. 
        /// </summary>
        /// <param name="depth">The depth of child (subsystem) to search for. Leave at -1 for no subsystem selection.</param>
        public virtual void SelectFront(int depth = -1)
        {
            Trackable frontTrackable = GetFrontMostTarget(transform.position, transform.forward, depth);
            if (frontTrackable != null)
            {
                float angle = Vector3.Angle(frontTrackable.transform.position - transform.position, transform.forward);
                if (angle < frontTargetAngle)
                {
                    SetSelected(frontTrackable);
                }
            }
        }


        /// <summary>
        /// Get the front most target within the front target angle limits.
        /// </summary>
        /// <param name="position">The reference position.</param>
        /// <param name="forwardDirection">The forward direction of the vehicle.</param>
        /// <param name="depth">What depth of child trackable (subsystem) to search for. Leave at -1 for no child target (subsystem) selection.</param>
        public virtual void SelectFront(Vector3 position, Vector3 forwardDirection, int depth = -1)
        {
            Trackable frontTrackable = GetFrontMostTarget(position, forwardDirection, depth);
            if (frontTrackable != null)
            {
                float angle = Vector3.Angle(frontTrackable.transform.position - position, forwardDirection);
                if (angle < frontTargetAngle)
                {
                    SetSelected(frontTrackable);
                }
            }
        }


        public virtual void SelectRandom(int depth = -1)
        {
            List<int> indexes = new List<int>();

            for (int i = 0; i < trackables.Count; ++i)
            {
                if (depth != -1 && trackables[i].Depth != depth) continue;

                if (IsSelectable(trackables[i]))
                {
                    indexes.Add(i);
                }
            }

            if (indexes.Count > 0)
            {
                SetSelected(trackables[indexes[Random.Range(0, indexes.Count)]]);
            }
        }


        protected virtual void AimTargetSelectionUpdate()
        {
            if (aimTargetSelection && aimController != null)
            {
                int currentIndex = trackables.IndexOf(selectedTarget);
                int nextIndex = -1;
                float minAngle = 180;
                for (int i = 0; i < trackables.Count; ++i)
                {
                    Vector3 toTarget = trackables[i].transform.position - aimController.Aim.origin;
                    float angleToTarget = Vector3.Angle(toTarget, aimController.Aim.direction);

                    if (angleToTarget < aimTargetSelectionAngle && angleToTarget < minAngle)
                    {
                        nextIndex = i;
                        minAngle = angleToTarget;
                    }
                }

                if (nextIndex != -1 && nextIndex != currentIndex)
                {
                    Select(trackables[nextIndex]);
                }
            }
        }


        public virtual void UpdateTarget(TargetSelectionMode mode)
        {
            switch (mode)
            {
                case TargetSelectionMode.Next:

                    if (selectedTarget == null)
                    {
                        SelectFirstSelectableTarget(defaultDepth);
                    }
                    else
                    {
                        Cycle(true);
                    }

                    break;

                case TargetSelectionMode.Previous:

                    if (selectedTarget == null)
                    {
                        SelectFirstSelectableTarget(defaultDepth);
                    }
                    else
                    {
                        Cycle(false);
                    }

                    break;

                case TargetSelectionMode.Nearest:

                    SelectNearest(defaultDepth);

                    break;

                case TargetSelectionMode.Front:

                    Trackable frontMostTarget = GetFrontMostTarget(transform.position, transform.forward, defaultDepth);
                    if (frontMostTarget != null) SetSelected(frontMostTarget);

                    break;

                case TargetSelectionMode.Random:

                    SelectRandom(defaultDepth);

                    break;
            }
        }


        // Called every frame
        protected virtual void Update()
        {
            AimTargetSelectionUpdate();

            // If toggled, always look for a new target when none is selected
            if (scanEveryFrame && selectedTarget == null)
            {
                UpdateTarget(defaultTargetSelectionMode);
            }
        }
    }
}
