using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;

public static class TypingTestSetup
{
    [MenuItem("Tools/Setup Typing Test Scene")]
    static void SetupScene()
    {
        // ── Clean up ───────────────────────────────────────────────────
        foreach (string n in new[] { "Canvas", "GameManager", "EventSystem" })
        {
            var existing = GameObject.Find(n);
            if (existing != null) Object.DestroyImmediate(existing);
        }

        // ── Canvas ─────────────────────────────────────────────────────
        var canvasGO = new GameObject("Canvas");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // ── Background ─────────────────────────────────────────────────
        var bg = MakePanel(canvasGO.transform, "Background",
            stretch: true, color: new Color(0.08f, 0.08f, 0.14f));

        // ── Title ──────────────────────────────────────────────────────
        MakeText(bg.transform, "TitleText", "TYPING SPEED TEST",
            anchor: new Vector2(0.5f, 1f), pos: new Vector2(0, -60), size: new Vector2(800, 80),
            fontSize: 48, style: FontStyles.Bold, color: Color.white, align: TextAlignmentOptions.Center);

        // ── Prompt text ────────────────────────────────────────────────
        var promptText = MakeText(bg.transform, "PromptText", "",
            anchor: new Vector2(0.5f, 0.5f), pos: new Vector2(0, 80), size: new Vector2(1100, 200),
            fontSize: 36, style: FontStyles.Normal, color: Color.white, align: TextAlignmentOptions.Center);
        promptText.textWrappingMode = TextWrappingModes.Normal;

        // ── Invisible input field ──────────────────────────────────────
        var inputGO = new GameObject("TypingInput");
        inputGO.transform.SetParent(bg.transform, false);
        var inputRT = inputGO.AddComponent<RectTransform>();
        inputRT.anchorMin        = new Vector2(0.5f, 0.5f);
        inputRT.anchorMax        = new Vector2(0.5f, 0.5f);
        inputRT.anchoredPosition = new Vector2(0, -80);
        inputRT.sizeDelta        = new Vector2(1100, 60);

        var inputImg = inputGO.AddComponent<Image>();
        inputImg.color = new Color(0.12f, 0.12f, 0.2f);

        var inputField = inputGO.AddComponent<TMP_InputField>();

        // Input field needs a text area child
        var textAreaGO = new GameObject("Text Area");
        textAreaGO.transform.SetParent(inputGO.transform, false);
        var taRT = textAreaGO.AddComponent<RectTransform>();
        taRT.anchorMin = Vector2.zero;
        taRT.anchorMax = Vector2.one;
        taRT.offsetMin = new Vector2(10, 5);
        taRT.offsetMax = new Vector2(-10, -5);
        textAreaGO.AddComponent<RectMask2D>();

        var inputTextGO = new GameObject("Text");
        inputTextGO.transform.SetParent(textAreaGO.transform, false);
        var itRT = inputTextGO.AddComponent<RectTransform>();
        itRT.anchorMin = Vector2.zero;
        itRT.anchorMax = Vector2.one;
        itRT.offsetMin = Vector2.zero;
        itRT.offsetMax = Vector2.zero;
        var inputTMP = inputTextGO.AddComponent<TextMeshProUGUI>();
        inputTMP.fontSize  = 28;
        inputTMP.color     = new Color(0.9f, 0.9f, 0.9f);
        inputTMP.alignment = TextAlignmentOptions.Left;

        var placeholderGO = new GameObject("Placeholder");
        placeholderGO.transform.SetParent(textAreaGO.transform, false);
        var phRT = placeholderGO.AddComponent<RectTransform>();
        phRT.anchorMin = Vector2.zero;
        phRT.anchorMax = Vector2.one;
        phRT.offsetMin = Vector2.zero;
        phRT.offsetMax = Vector2.zero;
        var phTMP = placeholderGO.AddComponent<TextMeshProUGUI>();
        phTMP.text      = "Type here...";
        phTMP.fontSize  = 28;
        phTMP.color     = new Color(0.4f, 0.4f, 0.5f);
        phTMP.alignment = TextAlignmentOptions.Left;
        phTMP.fontStyle = FontStyles.Italic;

        inputField.textViewport   = taRT;
        inputField.textComponent  = inputTMP;
        inputField.placeholder    = phTMP;
        inputField.caretWidth     = 2;
        inputField.caretColor     = Color.white;

        // ── Message text ───────────────────────────────────────────────
        var messageText = MakeText(bg.transform, "MessageText", "Start typing...",
            anchor: new Vector2(0.5f, 0.5f), pos: new Vector2(0, -180), size: new Vector2(900, 60),
            fontSize: 30, style: FontStyles.Bold, color: new Color(0.8f, 0.8f, 0.8f), align: TextAlignmentOptions.Center);

        // ── Restart button ─────────────────────────────────────────────
        var restartBtn = MakeButton(bg.transform, "RestartButton", "RESTART",
            anchor: new Vector2(0.5f, 0f), pos: new Vector2(0, 80), size: new Vector2(220, 60),
            bgColor: new Color(0.18f, 0.18f, 0.32f));

        // ── EventSystem ────────────────────────────────────────────────
        var esGO = new GameObject("EventSystem");
        esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        var inputModule = esGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        // ── GameManager ────────────────────────────────────────────────
        var managerGO = new GameObject("GameManager");
        var typingTest = managerGO.AddComponent<TypingTest>();

        typingTest.promptText  = promptText;
        typingTest.messageText = messageText;
        typingTest.typingInput = inputField;
        typingTest.restartButton = restartBtn.GetComponent<Button>();

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Selection.activeGameObject = managerGO;

        Debug.Log("✅ Typing Test scene built! Press Play to test.");
    }

    // ── Helpers ────────────────────────────────────────────────────────

    static GameObject MakePanel(Transform parent, string name, bool stretch,
        Vector2 anchor = default, Vector2 pos = default, Vector2 size = default, Color color = default)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        if (stretch)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        else
        {
            rt.anchorMin        = anchor;
            rt.anchorMax        = anchor;
            rt.pivot            = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = pos;
            rt.sizeDelta        = size;
        }
        var img = go.AddComponent<Image>();
        img.color = color == default ? Color.white : color;
        return go;
    }

    static TextMeshProUGUI MakeText(Transform parent, string name, string text,
        Vector2 anchor, Vector2 pos, Vector2 size,
        float fontSize, FontStyles style, Color color, TextAlignmentOptions align)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin        = anchor;
        rt.anchorMax        = anchor;
        rt.pivot            = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text      = text;
        tmp.fontSize  = fontSize;
        tmp.fontStyle = style;
        tmp.color     = color;
        tmp.alignment = align;
        tmp.richText  = true;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        return tmp;
    }

    static GameObject MakeButton(Transform parent, string name, string label,
        Vector2 anchor, Vector2 pos, Vector2 size, Color bgColor)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin        = anchor;
        rt.anchorMax        = anchor;
        rt.pivot            = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta        = size;
        var img = go.AddComponent<Image>();
        img.color = bgColor;
        var btn = go.AddComponent<Button>();
        var cb  = btn.colors;
        cb.normalColor      = bgColor;
        cb.highlightedColor = bgColor * 1.35f;
        cb.pressedColor     = bgColor * 0.65f;
        btn.colors = cb;

        MakeText(go.transform, "Text", label,
            anchor: new Vector2(0.5f, 0.5f), pos: Vector2.zero, size: size,
            fontSize: 22, style: FontStyles.Bold, color: Color.white, align: TextAlignmentOptions.Center);

        return go;
    }
}
