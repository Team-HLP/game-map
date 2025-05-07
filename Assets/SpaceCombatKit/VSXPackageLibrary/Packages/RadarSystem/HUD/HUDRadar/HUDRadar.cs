using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VSX.Teams;
using VSX.HUD;

namespace VSX.RadarSystem
{

    /// <summary>
    /// Shows a 2D or 3D radar on a vehicle HUD.
    /// </summary>
    public class HUDRadar : HUDComponent
    {

        [Header("Target Information Sources")]


        [Tooltip("The trackers (radars) that provide target information for the HUD.")]
        [SerializeField]
        protected List<Tracker> trackers = new List<Tracker>();
        public List<Tracker> Trackers
        {
            get { return trackers; }
        }


        [Header("Widgets")]


        [Tooltip("The parent transform for the radar widgets/icons.")]
        [SerializeField]
        protected Transform widgetParent;


        [Tooltip("The radar widget that is used by default for all target types.")]
        [SerializeField]
        protected HUDRadarWidget defaultRadarWidget;
        protected HUDRadarWidgetContainer defaultRadarWidgetContainer;


        [Tooltip("A list of radar widget prefabs that are to be used for specific target types.")]
        [SerializeField]
        protected List<HUDRadarWidgetContainer> radarWidgetOverrides = new List<HUDRadarWidgetContainer>();


        [Header("Settings")]


        [Tooltip("The radar widget scale.")]
        [SerializeField]
        protected float widgetScale = 1;


        [Tooltip("Whether to clamp the radar widgets to the border, or have them disappear when they reach the edge.")]
        [SerializeField]
        protected bool clampToBorder = false;


        [Tooltip("The 2D/3D radar radius on the HUD.")]
        [SerializeField]
        protected float equatorRadius = 0.5f;
        public float EquatorRadius { get => equatorRadius; }


        [Tooltip("Use an exponential function for displaying target distance - a higher exponent means that a larger area of the radar is used for closer proximity targets.")]
        [SerializeField]
        protected float scaleExponent = 1f;


        [Tooltip("The current zoom level of the HUD radar.")]
        protected float currentZoom = 1;
        public float CurrentZoom { get { return currentZoom; } }


        // The current display range of the HUD radar, as a function of the current zoom.
        protected float displayRange = 0;
        public float DisplayRange { get { return displayRange; } }


        [Tooltip("Limit the max number of radar widgets added per frame, useful for improving performance when radar is switched on.")]
        [SerializeField]
        protected int maxNewTargetsEachFrame = 1;
        protected int numTargetsLastFrame;
        protected int displayedTargetCount;


        [Tooltip("The default color used for the radar widgets.")]
        [SerializeField]
        protected Color defaultWidgetColor = Color.white;


        [Tooltip("The colors used for radar widgets representing different teams. Leave this empty if you wish to use the Team colors set on the Team object.")]
        [SerializeField]
        protected List<TeamColor> teamColors = new List<TeamColor>();


        [Tooltip("Whether the radar is to display in 3D (normal orientation) or 2D (top down).")]
        [SerializeField]
        protected bool display2D = false;


        protected Vector3 targetRelPos; // Cache a variable that's used for every target, for performance



        // Called when the component is first added to a gameobject, or reset in the inspector.
        protected virtual void Reset()
        {
            Tracker[] trackersArray = transform.root.GetComponentsInChildren<Tracker>();
            foreach (Tracker tracker in trackersArray)
            {
                trackers.Add(tracker);
            }

            widgetParent = transform;
        }


        // Called when something is changed in the inspector.
        protected virtual void OnValidate()
        {
            // Make sure that the assigned exponent is greater or equal to 1
            scaleExponent = Mathf.Max(scaleExponent, 1);
        }


        protected override void Awake()
        {

            base.Awake();

            if (defaultRadarWidget != null)
            {
                defaultRadarWidgetContainer = new HUDRadarWidgetContainer(defaultRadarWidget);
            }

            if (m_HUDCamera == null) m_HUDCamera = Camera.main;

            // Make sure that the distance exponent is at least 1
            scaleExponent = Mathf.Max(scaleExponent, 1);

            currentZoom = 0f;
        }


        protected virtual void Start()
        {
            UpdateDisplayRange();
        }


        /// <summary>
        /// Set the zoom.
        /// </summary>
        /// <param name="zoom">The zoom.</param>
        public virtual void SetZoom(float zoom)
        {
            // The zoom must be greater than 0
            currentZoom = Mathf.Max(zoom, 0);
        }


        // Visualize a target tracked by a Tracker component
        protected virtual void Visualize(Tracker tracker, Trackable trackable)
        {

            targetRelPos = tracker.ReferenceTransform.InverseTransformPoint(trackable.transform.position);

            // If target is outside the radar display range and is not clamped to the border, ignore it
            if (targetRelPos.magnitude > displayRange)
            {
                if (!clampToBorder)
                {
                    return;
                }
            }

            // Get a radar widget that matches the trackable type and display it
            HUDRadarWidget radarWidget = null;
            for (int i = 0; i < radarWidgetOverrides.Count; ++i)
            {
                for (int j = 0; j < radarWidgetOverrides[i].trackableTypes.Count; ++j)
                {
                    if (radarWidgetOverrides[i].trackableTypes[j] == trackable.TrackableType)
                    {
                        radarWidget = radarWidgetOverrides[i].GetNextAvailable(widgetParent);
                        break;
                    }
                }

                if (radarWidget != null) break;
            }

            if (radarWidget == null && defaultRadarWidgetContainer != null)
            {
                radarWidget = defaultRadarWidgetContainer.GetNextAvailable(widgetParent);
            }

            if (defaultRadarWidget == null) return;

            // Update the color of the target box
            if (trackable.Team != null)
            {
                radarWidget.SetColor(trackable.Team.DefaultColor);
                for (int i = 0; i < teamColors.Count; ++i)
                {
                    if (teamColors[i].team == trackable.Team)
                    {
                        radarWidget.SetColor(teamColors[i].color);
                    }
                }
            }
            else
            {
                radarWidget.SetColor(defaultWidgetColor);
            }

            bool isSelected = false;
            for (int i = 0; i < tracker.TargetSelectors.Count; ++i)
            {
                if (tracker.TargetSelectors[i].SelectedTarget == trackable)
                {
                    isSelected = true;
                    break;
                }
            }
            radarWidget.SetSelected(isSelected);

            Vector3 localPos;
            if (targetRelPos.magnitude > displayRange)
            {
                localPos = (targetRelPos.normalized * displayRange) * (equatorRadius / displayRange);

                radarWidget.SetIsClampedToBorder(true);
            }
            else
            {

                float amount = (targetRelPos.magnitude / displayRange);
                amount = 1 - Mathf.Pow(1 - amount, scaleExponent);
                localPos = (amount * equatorRadius) * targetRelPos.normalized;

                radarWidget.SetIsClampedToBorder(false);
            }

            if (display2D)
            {
                localPos.y = localPos.z;
                localPos.z = 0;
            }

            radarWidget.transform.localPosition = localPos;

            radarWidget.transform.localRotation = Quaternion.identity;

            radarWidget.transform.localScale = new Vector3(widgetScale, widgetScale, widgetScale);

            radarWidget.UpdateRadarWidget(trackable);

            return;
        }


        // Late update
        protected virtual void LateUpdate()
        {
            // If not activated, clear all the radar widgets and exit
            if (!activated)
            {
                for (int i = 0; i < radarWidgetOverrides.Count; ++i)
                {
                    radarWidgetOverrides[i].Begin();
                    radarWidgetOverrides[i].Finish();

                    if (defaultRadarWidgetContainer != null)
                    {
                        defaultRadarWidgetContainer.Begin();
                        defaultRadarWidgetContainer.Finish();
                    }

                }
                return;
            }

            UpdateDisplayRange();

            // Begin using the radar widget containers
            if (defaultRadarWidgetContainer != null) defaultRadarWidgetContainer.Begin();
            for (int i = 0; i < radarWidgetOverrides.Count; ++i)
            {
                radarWidgetOverrides[i].Begin();
            }

            // Visualize the targets
            for (int i = 0; i < trackers.Count; ++i)
            {
                for (int j = 0; j < trackers[i].Targets.Count; ++j)
                {

                    Visualize(trackers[i], trackers[i].Targets[j]);

                    // Don't add more than the specified amount of widgets per frame
                    if (displayedTargetCount - numTargetsLastFrame >= maxNewTargetsEachFrame)
                    {
                        break;
                    }
                }
            }

            numTargetsLastFrame = displayedTargetCount;

            // Finish using the radar widget containers
            if (defaultRadarWidgetContainer != null) defaultRadarWidgetContainer.Finish();
            for (int i = 0; i < radarWidgetOverrides.Count; ++i)
            {
                radarWidgetOverrides[i].Finish();
            }
        }


        // Update the current display range of the HUD radar, as a function of the current zoom level.
        protected virtual void UpdateDisplayRange()
        {
            // Update the display range
            displayRange = 0;
            for (int i = 0; i < trackers.Count; ++i)
            {
                displayRange = Mathf.Max(displayRange, trackers[i].Range);
            }
            displayRange = (1 - currentZoom) * displayRange;
        }


        protected virtual void OnDrawGizmosSelected()
        {
            if (widgetParent != null)
            {
                Gizmos.DrawWireSphere(widgetParent.position, equatorRadius * transform.localScale.x);
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, equatorRadius * transform.localScale.x);
            }
        }
    }
}
