using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace VSX.UI
{
    /// <summary>
    /// The Button Controller provides an alternative to Unity's Button component with more functionality, including the ability to Deep Select the button and also control
    /// images and texts in different button states.
    /// </summary>
    public class ButtonController : Button, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler, ISubmitHandler
    {
        // Used to identify the button
        protected int m_ID;
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        [Tooltip("The images on this button, can be accessed/set externally by index.")]
        [SerializeField]
        protected List<Image> images = new List<Image>();

        [Tooltip("The texts on this button, can be accessed/set externally by index.")]
        [SerializeField]
        protected List<UVCText> texts = new List<UVCText>();

        [Tooltip("Whether the button should be selected manually (via script), rather than being selected by pointer events.")]
        [SerializeField]
        protected bool selectManually = false;

        [Tooltip("Whether to reset the button state to Normal when it is enabled in the scene.")]
        [SerializeField]
        protected bool resetStateOnEnable = true;


        [Header("States")]

        [Tooltip("The normal state settings for the button.")]
        [SerializeField]
        protected ButtonState normalState;

        [Tooltip("The highlighted state settings for the button.")]
        [SerializeField]
        protected ButtonState highlightedState;

        [Tooltip("The selected state settings for the button.")]
        [SerializeField]
        protected ButtonState selectedState;

        [Tooltip("The deep selected state settings for the button.")]
        [SerializeField]
        protected ButtonState deepSelectedState;

        protected ButtonState currentState;

        [Header("Transition")]

        [Tooltip("How long the transition between states should take.")]
        [SerializeField]
        protected float transitionDuration = 0.1f;

        [Tooltip("The curve describing how the transition occurs from the starting state to the target state during the transition.")]
        [SerializeField]
        protected AnimationCurve transitionCurve = AnimationCurve.Linear(0, 0, 1, 1);

        protected Coroutine transitionCoroutine;

        protected object targetObject;
        public object TargetObject { get { return targetObject; } }

        public UnityAction<object> onTargetObjectChanged;


#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            normalState = new ButtonState("Normal");

            highlightedState = new ButtonState("Highlighted");
            highlightedState.priority = 1;

            selectedState = new ButtonState("Selected");
            selectedState.priority = 2;

            deepSelectedState = new ButtonState("Deep Selected");

            images = new List<Image>(GetComponentsInChildren<Image>());
            texts = new List<UVCText>(GetComponentsInChildren<UVCText>());

            transition = Transition.None;
        }
#endif


        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                if (normalState != null)
                {
                    normalState.StateActive = true;    // Needs null check as Button is executeineditmode
                    SetState(GetPriorityState());
                }
            }
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            if (Application.isPlaying)
            {
                if (normalState != null)
                {
                    if (resetStateOnEnable)
                    {
                        highlightedState.StateActive = false;
                        selectedState.StateActive = false;
                        SetState(normalState);
                    }
                    else
                    {
                        SetState(currentState); // Necessary in case coroutine was cut off during last disable
                    }
                }
            }
        }


        /// <summary>
        /// Set an object that defines what the button is related to, so that information about what the button refers to can be accessed from it.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        public virtual void SetTargetObject(object targetObject)
        {
            this.targetObject = targetObject;
            if (onTargetObjectChanged != null) onTargetObjectChanged.Invoke(targetObject);
        }


        /// <summary>
        /// Called when a pointer enters the button as a result of UI events.
        /// </summary>
        /// <param name="eventData">The pointer event data.</param>
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            SetHighlighted(true);
        }


        /// <summary>
        /// Called when a pointer exits the button as a result of UI events.
        /// </summary>
        /// <param name="eventData">The pointer event data.</param>
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            SetHighlighted(false);
        }


        /// <summary>
        /// Called when the button is selected by UI events.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public override void OnSelect(BaseEventData eventData)
        {
            if (!selectManually)
            {
                base.OnSelect(eventData);
                SetSelected(true);
            }
        }


        /// <summary>
        /// Called when the button is deselected by UI events.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            if (!selectManually) SetSelected(false);
        }

        public virtual void SetNormal(bool isNormal)
        {
            normalState.StateActive = isNormal;
            UpdateState();
        }

        /// <summary>
        /// Set whether the button is highlighted.
        /// </summary>
        /// <param name="isHighlighted">Whether the button is highlighted or not.</param>
        public virtual void SetHighlighted(bool isHighlighted)
        {
            highlightedState.StateActive = isHighlighted;
            UpdateState();
        }


        /// <summary>
        /// Set whether the button is selected.
        /// </summary>
        /// <param name="isSelected">Whether the button is selected or not.</param>
        public virtual void SetSelected(bool isSelected)
        {
            selectedState.StateActive = isSelected;
            UpdateState();
        }


        /// <summary>
        /// Set whether the button is deep selected.
        /// </summary>
        /// <param name="isDeepSelected">Whether the button is deep selected or not.</param>
        public virtual void SetDeepSelected(bool isDeepSelected)
        {
            deepSelectedState.StateActive = isDeepSelected;
            UpdateState();
        }


        /// <summary>
        /// Update the button state.
        /// </summary>
        protected virtual void UpdateState()
        {

            // Set/transition to the highest priority active state
            ButtonState priorityState = GetPriorityState();

            if (priorityState != currentState)
            {
                if (gameObject.activeInHierarchy)
                {
                    TransitionTo(priorityState);
                }
                else
                {
                    SetState(priorityState);
                }

                normalState.IsPriorityState = priorityState == normalState;
                highlightedState.IsPriorityState = priorityState == normalState;
                selectedState.IsPriorityState = priorityState == normalState;
                deepSelectedState.IsPriorityState = priorityState == normalState;
            }
        }


        // Get the highest priority active state of the button
        protected virtual ButtonState GetPriorityState()
        {
            ButtonState result = normalState;

            if (highlightedState.StateActive && highlightedState.priority > result.priority)
            {
                result = highlightedState;
            }

            if (selectedState.StateActive && selectedState.priority > result.priority)
            {
                result = selectedState;
            }

            if (deepSelectedState.StateActive && deepSelectedState.priority > result.priority)
            {
                result = deepSelectedState;
            }

            return result;
        }


        /// <summary>
        /// Transition to a new state.
        /// </summary>
        /// <param name="targetState">The state to transition to.</param>
        public virtual void TransitionTo(ButtonState targetState)
        {
            if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
            transitionCoroutine = StartCoroutine(TransitionCoroutine(targetState));
        }


        // The coroutine for transitioning to a new state
        protected virtual IEnumerator TransitionCoroutine(ButtonState targetState)
        {
            currentState = targetState;

            float startTime = Time.unscaledTime;

            List<Color> textPropertyStartColors = new List<Color>();
            foreach (ButtonState.TextSetting textProperty in targetState.textSettings)
            {
                textPropertyStartColors.Add(textProperty.text.color);
            }

            List<Color> imagePropertyStartColors = new List<Color>();
            foreach (ButtonState.ImageSetting imageProperty in targetState.imageSettings)
            {
                imagePropertyStartColors.Add(imageProperty.image.color);
            }

            while (Time.unscaledTime - startTime < transitionDuration)
            {
                float amount = transitionCurve.Evaluate((Time.unscaledTime - startTime) / transitionDuration);

                for (int i = 0; i < targetState.textSettings.Count; ++i)
                {
                    if (targetState.textSettings[i].modifyColor)
                    {
                        targetState.textSettings[i].text.color = Color.Lerp(textPropertyStartColors[i], targetState.textSettings[i].color, amount);
                    }
                }

                for (int i = 0; i < targetState.imageSettings.Count; ++i)
                {
                    if (targetState.imageSettings[i].modifyColor)
                    {
                        targetState.imageSettings[i].image.color = Color.Lerp(imagePropertyStartColors[i], targetState.imageSettings[i].color, amount);
                    }
                }

                yield return null;
            }

            SetState(targetState);
        }


        /// <summary>
        /// Set the button state.
        /// </summary>
        /// <param name="newState">The new state.</param>
        public virtual void SetState(ButtonState newState)
        {
            foreach (ButtonState.TextSetting textProperty in newState.textSettings)
            {
                if (textProperty.modifyColor)
                {
                    textProperty.text.color = textProperty.color;
                }
            }

            foreach (ButtonState.ImageSetting imageProperty in newState.imageSettings)
            {
                if (imageProperty.modifyColor)
                {
                    imageProperty.image.color = imageProperty.color;
                }
            }

            currentState = newState;
        }


        /// <summary>
        /// Set an image on the button by index.
        /// </summary>
        /// <param name="index">The image index within the Images list.</param>
        /// <param name="sprite">The sprite for the image.</param>
        public virtual void SetImage(int index, Sprite sprite)
        {
            if (index >= 0 && index < images.Count)
            {
                images[index].sprite = sprite;
            }
        }


        /// <summary>
        /// Set a text on the button by index.
        /// </summary>
        /// <param name="index">The index of the text within the Texts list.</param>
        /// <param name="content">The text content.</param>
        public virtual void SetText(int index, string content)
        {
            if (index >= 0 && index < texts.Count)
            {
                texts[index].text = content;
            }
        }
    }
}