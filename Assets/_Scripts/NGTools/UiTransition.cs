using UnityEngine;

namespace NG.UI
{
    public class UiTransition : MonoBehaviour
    {
        #region Inspector
        public bool startActive = false;
        public enum UiTransitionType { Slide, Scale, Fade }
        public UiTransitionType transitionType = UiTransitionType.Fade;

        public enum ScaleType { XY, X, Y}
        public ScaleType scalingType = ScaleType.XY;

        public enum SlideInFrom { Top, Right, Bottom, Left }
        public SlideInFrom slideInFrom = SlideInFrom.Top;

        public float duration = 0.25f;

        [SerializeField]
        private RectTransform mainCanvasRectTransform = null;
        private RectTransform MainCanvasRectTransform
        {
            get
            {
                if (mainCanvasRectTransform == null)
                {
                    Canvas mainCanvas = gameObject.GetComponentInParent<Canvas>();
                    if (mainCanvas == null)
                        throw new MissingComponentException("Cannot find the main canvas that this UI element is a child of. Please assign it via the inspector.");

                    mainCanvasRectTransform = mainCanvas.GetComponent<RectTransform>();
                }

                return mainCanvasRectTransform;
            }
        }
        #endregion

        public bool IsActive { get; private set; }

        private CanvasGroup _canvasGroup = null;
        private CanvasGroup canvasGroup
        {
            get
            {
                if (_canvasGroup == null)
                {
                    _canvasGroup = gameObject.GetOrCreateComponent<CanvasGroup>();
                    SetActive(startActive);
                    canvasGroup.alpha = (startActive ? 1 : 0);
                }

                return _canvasGroup;
            }
        }

        private RectTransform _rectTransform = null;
        public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = gameObject.GetComponent<RectTransform>();

                return _rectTransform;
            }

        }

        private enum TransitionState { Opening, Open, Closing, Closed}
        private TransitionState transitionState = TransitionState.Closed;

        private float transitionStartTime = float.NaN;

        private float alphaAtToggle = 0f;

        private Vector3 initialPosition;
        private Vector3 positionAtToggle = Vector3.zero;
        private Vector3 offScreenPosition { get { return GetOffScreenPosition(); } }
        private float fullSlideDistance = 0f;
        private float slideDistanceAtToggle = 0f;

        private Vector3 initialScale;
        private Vector3 closedScale
        {
            get
            {
                Vector3 scale = Vector3.one;
                switch (scalingType)
                {
                    case ScaleType.XY:
                        scale.x = 0;
                        scale.y = 0;
                        break;
                    case ScaleType.X:
                        scale.x = 0;
                        break;
                    case ScaleType.Y:
                        scale.y = 0;
                        break;
                }

                return scale;
            }
        }
        private Vector3 scaleAtToggle = Vector3.one;
        private float fullScaleDistance = 0f;
        private float scaleDistanceAtToggle = 0f;


        #region Unity Messages
        private void Start()
        {
            initialPosition = rectTransform.localPosition;
            initialScale = gameObject.transform.localScale;

            SetActive(startActive);

            if (startActive)
            {
                transitionState = TransitionState.Open;
            }
            else
            {
                if (transitionType == UiTransitionType.Scale)
                    gameObject.transform.localScale = closedScale;

                if (transitionType == UiTransitionType.Slide)
                    rectTransform.localPosition = offScreenPosition;

                transitionState = TransitionState.Closed;
            }

            fullSlideDistance = (initialPosition - offScreenPosition).magnitude;
            fullScaleDistance = (initialScale - closedScale).magnitude;
        }
        
        private void Update()
        {
            HandleTransition(ref transitionState);
        }
        #endregion


        private void SetActive(bool active)
        {
            canvasGroup.interactable = active;
            canvasGroup.blocksRaycasts = active;
            IsActive = active;
        }

        
        public void Toggle()
        {
            if (transitionState == TransitionState.Open ||
                transitionState == TransitionState.Opening)
            {
                transitionState = TransitionState.Closing;
            }
            else
                transitionState = TransitionState.Opening;

            transitionStartTime = Time.time;

            if (transitionType == UiTransitionType.Fade)
                alphaAtToggle = canvasGroup.alpha;

            positionAtToggle = rectTransform.localPosition;
            Vector3 positionFinal = (transitionState == TransitionState.Opening ? initialPosition : offScreenPosition);
            slideDistanceAtToggle = (positionFinal - positionAtToggle).magnitude;
            
            scaleAtToggle = rectTransform.localScale;
            Vector3 scaleFinal = (transitionState == TransitionState.Opening ? initialScale : closedScale);
            scaleDistanceAtToggle = (scaleFinal - scaleAtToggle).magnitude;
        }
        

        private void HandleTransition(ref TransitionState transitionState)
        {
            switch (transitionState)
            {
                case TransitionState.Opening:
                    if (!IsActive)
                        SetActive(true);
                    
                    if (float.IsNaN(transitionStartTime))
                        transitionStartTime = Time.time;
                    
                    if (HandleTransition())
                        transitionState = TransitionState.Open;

                    break;
                case TransitionState.Open:
                    transitionStartTime = float.NaN;
                    break;
                case TransitionState.Closing:
                    if (float.IsNaN(transitionStartTime))
                        transitionStartTime = Time.time;

                    if (HandleTransition())
                        transitionState = TransitionState.Closed;
                    break;
                case TransitionState.Closed:
                    if (IsActive)
                        SetActive(false);
                    break;
            }
        }


        private bool HandleTransition()
        {
            bool isDone = false;
            float elapsedTime = Time.time - transitionStartTime;
            

            switch (transitionType)
            {
                case UiTransitionType.Slide:
                    isDone = Slide(elapsedTime);
                    break;
                case UiTransitionType.Scale:
                    isDone = Scale(elapsedTime);
                    break;
                case UiTransitionType.Fade:
                    isDone = Fade(elapsedTime);
                    break;
            }

            return isDone;
        }


        public Vector3 GetOffScreenPosition()
        {
            Vector2 pivot = rectTransform.pivot;
            Vector3 position = rectTransform.localPosition;
            switch (slideInFrom)
            {
                case SlideInFrom.Top:
                    position.y = 
                        0.5f * MainCanvasRectTransform.sizeDelta.y + pivot.y * rectTransform.sizeDelta.y;
                    break;
                case SlideInFrom.Bottom:
                    position.y = 
                        -1f *(0.5f * MainCanvasRectTransform.sizeDelta.y + pivot.y * rectTransform.sizeDelta.y);
                    break;
                case SlideInFrom.Right:
                    position.x = 
                        0.5f * MainCanvasRectTransform.sizeDelta.x + pivot.x * rectTransform.sizeDelta.x;
                    break;
                case SlideInFrom.Left:
                    position.x = 
                        -1f * (0.5f * MainCanvasRectTransform.sizeDelta.x + pivot.x * rectTransform.sizeDelta.x);
                    break;
            }

            return position;
        }


        private bool Scale(float elapsedTime)
        {
            bool isDone = false;
            canvasGroup.alpha = 1;
            Vector3 scaleStart = scaleAtToggle;
            Vector3 scaleFinal = (transitionState == TransitionState.Opening ? initialScale : closedScale);

            float realDuration = duration * (scaleDistanceAtToggle / fullScaleDistance);

            Vector3 scale = Vector3.Lerp(scaleStart, scaleFinal, elapsedTime / realDuration);

            if (elapsedTime >= realDuration || scale == scaleFinal)
            {
                scale = scaleFinal;
                isDone = true;
            }

            rectTransform.localScale = scale;

            return isDone;
        }


        private bool Slide(float elapsedTime)
        {
            bool isDone = false;
            canvasGroup.alpha = 1;
            Vector3 positionStart = positionAtToggle;
            Vector3 positionFinal = (transitionState == TransitionState.Opening ? initialPosition : offScreenPosition);

            float realDuration = duration * (slideDistanceAtToggle / fullSlideDistance);

            Vector3 position = Vector3.Lerp(positionStart, positionFinal, elapsedTime / realDuration);

            if (elapsedTime >= realDuration || position == positionFinal)
            {
                position = positionFinal;
                isDone = true;
            }

            rectTransform.localPosition = position;

            return isDone;
        }


        private bool Fade(float elapsedTime)
        {
            bool isDone = false;
            float alphaStart = alphaAtToggle;
            float alphaFinal = (transitionState == TransitionState.Opening ? 1 : 0);

            float realDuration = duration * (Mathf.Abs(alphaFinal - alphaStart) / 1f);

            float alpha = Mathf.Lerp(alphaStart, alphaFinal, elapsedTime / realDuration);

            if (elapsedTime >= realDuration || Mathf.Approximately(alpha, alphaFinal))
            {
                alpha = alphaFinal;
                isDone = true;
            }

            canvasGroup.alpha = alpha;

            return isDone;
        }
    }
}
