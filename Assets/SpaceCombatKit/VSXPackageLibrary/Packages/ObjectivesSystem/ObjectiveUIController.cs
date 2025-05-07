using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VSX.UI;

namespace VSX.Objectives
{
    /// <summary>
    /// Manages the UI for a single objective.
    /// </summary>
    public class ObjectiveUIController : MonoBehaviour
    {

        [Tooltip("The objective that this UI represents.")]
        [SerializeField] protected ObjectiveController objectiveController;
        public ObjectiveController ObjectiveController
        {
            get { return objectiveController; }
            set { objectiveController = value; }
        }

        [Tooltip("The color manager for the UI elements.")]
        [SerializeField] protected UIColorManager colorManager;

        [Tooltip("The color that represents an uncompleted objective (applied to UI elements such as text, icons etc).")]
        [SerializeField] protected Color uncompletedColor;

        [Tooltip("The color that represents a completed objective (applied to UI elements such as text, icons etc).")]
        [SerializeField] protected Color completedColor;

        [Tooltip("Game objects that are active when the objective is not completed.")]
        [SerializeField] protected List<GameObject> incompleteActivatedObjects = new List<GameObject>();

        [Tooltip("Game objects that are active when the objective is completed.")]
        [SerializeField] protected List<GameObject> completeActivatedObjects = new List<GameObject>();

        [Tooltip("Whether to display the fraction of subobjectives that are completed as part of the objective.")]
        [SerializeField] protected bool displaySubObjectivesCount = true;

        [Tooltip("The description text for the objective.")]
        [SerializeField] protected UVCText descriptionText;

        public UnityEvent onCompleted;

        public UnityEvent onReset;


        protected virtual void Awake()
        {
            if (objectiveController != null) SetObjective(objectiveController);
        }


        /// <summary>
        /// Set the objective to display.
        /// </summary>
        /// <param name="objectiveController">The objective to display.</param>
        public virtual void SetObjective(ObjectiveController objectiveController)
        {
            if (this.objectiveController != null)
            {
                this.objectiveController.onCompleted.RemoveListener(OnObjectiveCompleted);
                this.objectiveController.onReset.RemoveListener(OnObjectiveCompleted);
                this.objectiveController.onChanged.RemoveListener(UpdateUI);
            }

            this.objectiveController = objectiveController;

            objectiveController.onCompleted.AddListener(OnObjectiveCompleted);
            objectiveController.onReset.AddListener(OnObjectiveCompleted);
            objectiveController.onChanged.AddListener(UpdateUI);

            descriptionText.text = objectiveController.Description;

            OnObjectiveReset();
        }


        protected virtual void Start()
        {
            UpdateUI();
        }


        // Called when the objective is completed.
        protected virtual void OnObjectiveCompleted()
        {
            foreach (GameObject g in incompleteActivatedObjects)
            {
                g.SetActive(false);
            }
            foreach (GameObject g in completeActivatedObjects)
            {
                g.SetActive(true);
            }

            colorManager.SetColor(completedColor);

            onCompleted.Invoke();
        }


        // Called when the objective is reset.
        protected virtual void OnObjectiveReset()
        {
            foreach (GameObject g in incompleteActivatedObjects)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in completeActivatedObjects)
            {
                g.SetActive(false);
            }

            colorManager.SetColor(uncompletedColor);

            onReset.Invoke();
        }


        // Update the UI when something about the objective changes.
        protected virtual void UpdateUI()
        {
            if (displaySubObjectivesCount && objectiveController.NumSubObjectives() > 1)
            {
                descriptionText.text = objectiveController.Description + " (" + objectiveController.NumSubObjectivesCompleted().ToString() + "/" + objectiveController.NumSubObjectives().ToString() + ")";
            }
            else
            {
                descriptionText.text = objectiveController.Description;
            }
        }
    }
}
