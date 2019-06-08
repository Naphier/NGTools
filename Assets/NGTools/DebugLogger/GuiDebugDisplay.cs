using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace NG
{
    public class GuiDebugDisplay : MonoBehaviour
    {
        private static Color backgroundColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 100f / 255f);
        public static int maxNumberOfMessages = 50;

        private static GuiDebugDisplay m_instance;
        private GameObject canvas;
        private GameObject scrollView;
        private GameObject scrollViewContent;
        private RectTransform scrollViewContentRectTransform;

        public delegate void ButtonDelegate();
        public ButtonDelegate ScreenshotButtonCallback;
        public ButtonDelegate SaveLogButtonCallback;
        public ButtonDelegate ClearLogButtonCallback;

        public static GuiDebugDisplay instance
        {
            get
            {
                if (m_instance == null)
                {
                    GameObject go = new GameObject("GuiDebugDisplay");
                    DontDestroyOnLoad(go);
                    m_instance = go.AddComponent<GuiDebugDisplay>();
                    m_instance.InitGui();
                }
                return m_instance;
            }
        }


        void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
            }
            else
            {
                DebugConsole[] consoles = GameObject.FindObjectsOfType<DebugConsole>();
                for (int i = 0; i < consoles.Length; i++)
                {
                    if (consoles[i] != m_instance)
                        Destroy(consoles[i].gameObject);
                }
            }
        }


        void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.BackQuote))
                ToggleDisplay();
        }

        void InitGui()
        {
            CreateEventSystem();
            CreateGUI();
            MakeButtons();
            canvas.SetActive(false);
            DebugConsole.instance.isVisible = false;
        }

        void ToggleDisplay()
        {
            canvas.SetActive(!canvas.activeSelf);
        }


        void CreateEventSystem()
        {
            EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject go = new GameObject("EventSystem");
                DontDestroyOnLoad(go);
                go.AddComponent<EventSystem>();
                go.AddComponent<StandaloneInputModule>();
            }
        }

        void CreateGUI()
        {
            //make canvas
            canvas = new GameObject("GuiDebugDisplayCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas c = canvas.GetComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;
            canvas.transform.SetParent(gameObject.transform);

            //make scroll view and necessary children
            scrollView = new GameObject("Scroll View", typeof(CanvasRenderer), typeof(Image), typeof(ScrollRect));
            Image image = scrollView.GetComponent<Image>();
            image.color = backgroundColor;

            scrollView.transform.SetParent(canvas.transform);
            RectTransform rt = scrollView.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            //viewport
            GameObject viewPort = new GameObject("Viewport", typeof(Mask), typeof(CanvasRenderer), typeof(Image));
            viewPort.transform.SetParent(scrollView.transform);
            RectTransform vpRt = viewPort.GetComponent<RectTransform>();
            vpRt.pivot = Vector2.up;
            vpRt.anchorMin = Vector2.zero;
            vpRt.anchorMax = Vector2.one;
            vpRt.anchoredPosition = Vector2.up;
            vpRt.offsetMin = Vector2.zero;
            vpRt.offsetMax = Vector2.zero;
            Mask mask = viewPort.GetComponent<Mask>();
            mask.showMaskGraphic = false;

            //content
            scrollViewContent = new GameObject("Content", typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            scrollViewContent.transform.SetParent(viewPort.transform);
            scrollViewContentRectTransform = scrollViewContent.GetComponent<RectTransform>();
            scrollViewContentRectTransform.offsetMin = Vector2.zero;
            scrollViewContentRectTransform.offsetMax = new Vector2(0f, 100f);
            scrollViewContentRectTransform.anchorMin = Vector2.up;
            scrollViewContentRectTransform.anchorMax = Vector2.one;
            scrollViewContentRectTransform.pivot = Vector2.up;

            VerticalLayoutGroup vlg = scrollViewContent.GetComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(10, 0, 10, 0);
            vlg.spacing = 10;
            vlg.childAlignment = TextAnchor.UpperLeft;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = true;

            ContentSizeFitter cssf = scrollViewContent.GetComponent<ContentSizeFitter>();
            cssf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            cssf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            //set scroll rect settings
            ScrollRect scrollRect = scrollView.GetComponent<ScrollRect>();
            scrollRect.content = scrollViewContentRectTransform;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
            scrollRect.elasticity = 0.1f;
            scrollRect.inertia = true;
            scrollRect.decelerationRate = 0.135f;
            scrollRect.scrollSensitivity = 1f;
            scrollRect.viewport = vpRt;
        }

        private List<GameObject> textGameObjects = new List<GameObject>();
        public void AddMessage(string message, LogType logType)
        {

            int lastNum = textGameObjects.Count;
            if (textGameObjects.Count > maxNumberOfMessages)
            {
                for (int i = 0; i < textGameObjects.Count; i++)
                {
                    if (textGameObjects[i] == null)
                        continue;

                    if (textGameObjects[i].name == "0")
                        Destroy(textGameObjects[i]);
                    else
                    {
                        //move all the numbers names up
                        int thisNum = int.Parse(textGameObjects[i].name);
                        textGameObjects[i].name = (thisNum + 1).ToString();
                        lastNum = thisNum;
                    }
                }
            }

            //make the new text
            GameObject newTextGo = new GameObject(lastNum.ToString(), typeof(CanvasRenderer), typeof(Text), typeof(LayoutElement));
            newTextGo.transform.SetParent(scrollViewContent.transform);
            newTextGo.transform.SetAsLastSibling();
            newTextGo.GetComponent<RectTransform>().localScale = Vector3.one;
            Text text = newTextGo.GetComponent<Text>();
            TextSettings.SetTextSettings(text, logType, message);

            //might want to position to the last message instead.
            scrollViewContentRectTransform.anchoredPosition = Vector2.zero;

            textGameObjects.Add(newTextGo);
        }

        private GameObject saveLogButton;
        private GameObject screenShotButton;
        private GameObject clearLogButton;
        void MakeButtons()
        {
            saveLogButton = new GameObject("SaveLogButton", typeof(CanvasRenderer), typeof(Image), typeof(Button));
            saveLogButton.transform.SetParent(canvas.transform);
            saveLogButton.transform.SetAsLastSibling();
            ButtonSettings.SetButton(saveLogButton, "Save Log", () => { SaveLogButtonPressed(); });
            RectTransform rt = saveLogButton.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector3(-85f, 0f, 0f);
            rt.sizeDelta = new Vector2(80f, 30f);

            screenShotButton = new GameObject("ScreenshotButton", typeof(CanvasRenderer), typeof(Image), typeof(Button));
            screenShotButton.transform.SetParent(canvas.transform);
            screenShotButton.transform.SetAsLastSibling();
            ButtonSettings.SetButton(screenShotButton, "Take Screenshot", () => { ScreenShotButton(); });
            RectTransform rt1 = screenShotButton.GetComponent<RectTransform>();
            rt1.anchoredPosition = new Vector3(-170f, 0f, 0f);
            rt1.sizeDelta = new Vector2(110f, 30f);

            clearLogButton = new GameObject("ClearLogButton", typeof(CanvasRenderer), typeof(Image), typeof(Button));
            clearLogButton.transform.SetParent(canvas.transform);
            clearLogButton.transform.SetAsLastSibling();
            ButtonSettings.SetButton(clearLogButton, "Clear Log", () => { ClearLog(); });
            RectTransform rt2 = clearLogButton.GetComponent<RectTransform>();
            rt2.anchoredPosition = new Vector3(0f, 0f, 0f);
            rt2.sizeDelta = new Vector2(80f, 30f);

        }


        public bool saveLogButtonVisible = true;
        public bool screenShotButtonVisible = true;
        public bool clearLogButtonVisible = true;
        private bool m_isVisible = false;
        public bool isVisible
        {
            get { return m_isVisible; }

            set
            {
                m_isVisible = value;
                if (canvas != null)
                    canvas.SetActive(m_isVisible);

                if (saveLogButton != null)
                {
                    if (!saveLogButtonVisible)
                        saveLogButton.SetActive(false);
                    else
                        saveLogButton.SetActive(m_isVisible);
                }

                if (screenShotButton != null)
                {
                    if (!screenShotButtonVisible)
                        screenShotButton.SetActive(false);
                    else
                        screenShotButton.SetActive(m_isVisible);
                }

                if (clearLogButton != null)
                {
                    if (!clearLogButtonVisible)
                        clearLogButton.SetActive(false);
                    else
                        clearLogButton.SetActive(m_isVisible);
                }
            }
        }

        public bool shouldSaveWindowLog = true;
        public void SaveLogButtonPressed()
        {
            if (shouldSaveWindowLog)
            {
                string path = GetFileSavePath();
                string file = Application.productName + "_errorlog.txt";


                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path + "/" + file))
                {
                    for (int i = 0; i < textGameObjects.Count; i++)
                    {
                        Text text = textGameObjects[i].GetComponent<Text>();
                        if (text != null)
                        {
                            sw.WriteLine(text.text);
                        }
                    }
                    sw.Close();
                }
            }

            if (SaveLogButtonCallback != null)
                SaveLogButtonCallback();
        }

        public bool shouldTakeScreenShot = true;
        public void ScreenShotButton()
        {
            if (shouldTakeScreenShot)
            {
                string path = GetFileSavePath();
                string file = Application.productName + "_errorscreen.png";

                StartCoroutine(TakeScreenshot(path + "/" + file));
            }
            else
            {
                if (ScreenshotButtonCallback != null)
                    ScreenshotButtonCallback();
            }
        }


        IEnumerator TakeScreenshot(string pathfile)
        {
            bool wasVisible = DebugConsole.instance.isVisible;
            DebugConsole.instance.isVisible = false;
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(pathfile);
            yield return new WaitForEndOfFrame();
            DebugConsole.instance.isVisible = wasVisible;

            if (ScreenshotButtonCallback != null)
                ScreenshotButtonCallback();
        }


        private string GetFileSavePath()
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            return path;
        }

        private void ClearLog()
        {
            for (int i = 0; i < textGameObjects.Count; i++)
            {
                Destroy(textGameObjects[i]);
            }

            if (ClearLogButtonCallback != null)
                ClearLogButtonCallback();
        }

    }


    public class TextSettings
    {
        private static Color assert = Color.magenta;
        private static Color error = Color.red;
        private static Color warning = Color.yellow;
        private static Color log = Color.white;

        public static Color GetColorByLogType(LogType logType)
        {
            switch (logType)
            {
                case LogType.Error:
                    return error;
                case LogType.Assert:
                    return assert;
                case LogType.Warning:
                    return warning;
                case LogType.Log:
                    return log;
                case LogType.Exception:
                    return error;
                default:
                    return log;
            }
        }

        public static FontStyle fontStyle
        { get { return FontStyle.Normal; } }

        public static int fontSize
        { get { return 12; } }

        public static float lineSpacing
        { get { return 1; } }

        public static bool richText
        { get { return true; } }

        public static TextAnchor alignment
        { get { return TextAnchor.UpperLeft; } }

        public static HorizontalWrapMode horizontalOverflow
        { get { return HorizontalWrapMode.Wrap; } }

        public static VerticalWrapMode verticalOverflow
        { get { return VerticalWrapMode.Truncate; } }

        public static bool bestFit
        { get { return false; } }

        public static bool rayCastTarget
        { get { return true; } }

        public static void SetTextSettings(Text text, LogType logType, string message, Font font)
        {
            text.color = GetColorByLogType(logType);
            text.text = message;
            text.fontStyle = fontStyle;
            text.fontSize = fontSize;
            text.lineSpacing = lineSpacing;
            text.supportRichText = richText;
            text.alignment = alignment;
            text.horizontalOverflow = horizontalOverflow;
            text.verticalOverflow = verticalOverflow;
            text.resizeTextForBestFit = bestFit;
            text.raycastTarget = rayCastTarget;
            text.font = font;
        }

        public static void SetTextSettings(Text text, LogType logType, string message)
        { SetTextSettings(text, logType, message, Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font); }

        public static void SetTextSettings(Text text, LogType logType)
        { SetTextSettings(text, logType, "", Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font); }

        public static void SetTextSettings(Text text)
        { SetTextSettings(text, LogType.Log, "", Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font); }

    }


    public class ButtonSettings
    {
        public static void SetButton(GameObject buttonGo, string label, UnityAction buttonListener)
        {
            Image image = buttonGo.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = null;
                image.color = Color.white;
                image.raycastTarget = true;
            }

            Button button = buttonGo.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = true;
                button.targetGraphic = image;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(buttonListener);
            }

            RectTransform rt = buttonGo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.right;
            rt.anchorMax = Vector2.right;
            rt.pivot = Vector2.right;


            Text text = buttonGo.GetComponentInChildren<Text>();
            if (text == null)
            {
                GameObject textLabel = new GameObject("Text", typeof(CanvasRenderer), typeof(Text));
                textLabel.transform.SetParent(buttonGo.transform);
                text = textLabel.GetComponent<Text>();
                text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                text.fontStyle = FontStyle.Normal;
                text.fontSize = 14;
                text.supportRichText = true;
                text.alignment = TextAnchor.MiddleCenter;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.verticalOverflow = VerticalWrapMode.Truncate;
                text.resizeTextForBestFit = false;
                text.color = Color.black;
                text.raycastTarget = true;

                RectTransform textRt = textLabel.GetComponent<RectTransform>();
                textRt.anchoredPosition = Vector3.zero;
                textRt.anchorMin = Vector2.zero;
                textRt.anchorMax = Vector2.one;
                textRt.pivot = new Vector2(0.5f, 0.5f);
                textRt.offsetMin = Vector2.zero;
                textRt.offsetMax = Vector2.zero;
            }

            text.text = label;

        }
    }
}