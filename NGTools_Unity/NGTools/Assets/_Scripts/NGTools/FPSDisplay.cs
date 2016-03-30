using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays FPS on screen via uGUI text if provided,
/// or OnGUI GUI.Label if uGUI text component is not provided.
/// </summary>
public class FPSDisplay : MonoBehaviour
{
    public float updateInterval = 0.5F;
    public int onGuiFontSize = 20;
    public TextAnchor onGuiTextAnchor = TextAnchor.UpperLeft;

    public Text uiText;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    private string label = "";
    private Color color = Color.cyan;

    private GUIStyle _style = null;
    private GUIStyle Style
    {
        get
        {
            if (_style == null)
            {
                _style = new GUIStyle();
                _style.alignment = onGuiTextAnchor;
                _style.fontSize = onGuiFontSize;
                _style.normal.textColor = color;
            }

            return _style;
        }
    }

    private Rect Rect = new Rect(0, 0, Screen.width, Screen.height);

    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            label = string.Format("{0:F2} FPS", fps);
            SetColor(fps);
            SetGuiText(fps, label);

            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }

    void SetGuiText(float fps, string label)
    {
        if (uiText == null)
            return;

        uiText.text = label;
        uiText.color = color;
    }

    void SetColor(float fps)
    {
        if (fps < 30)
            color = Color.yellow;
        else if (fps < 10)
            color = Color.red;
        else
            color = Color.green;
    }



    void OnGUI()
    {
        if (uiText != null)
            return;

        GUI.Label(Rect, label, Style);
    }
}
