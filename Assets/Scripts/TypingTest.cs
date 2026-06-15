using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypingTest : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI messageText;
    public TMP_InputField  typingInput;
    public Button          restartButton;

    [Header("Passages")]
    public string[] passages =
    {
        "What is the question of the day my class period.",
        "Practice makes perfect when it comes to getting fast at skills.",
        "Is football better than baseball?  The answer is it depends on the person.",
        "Consistency and focus are the keys to mastering any skill."
    };

    private string  currentPassage;
    private float   startTime;
    private bool    started;
    private bool    finished;

    void Start()
    {
        restartButton.onClick.AddListener(NewGame);
        typingInput.onValueChanged.AddListener(OnInputChanged);
        NewGame();
    }

    public void NewGame()
    {
        currentPassage = passages[Random.Range(0, passages.Length)];
        started  = false;
        finished = false;

        typingInput.text = "";
        typingInput.interactable = true;
        messageText.text = "Start typing...";
        BuildColoredPrompt("");

        typingInput.ActivateInputField();
    }

    void OnInputChanged(string typed)
    {
        if (finished) return;

        if (!started && typed.Length > 0)
        {
            started   = true;
            startTime = Time.time;
        }

        BuildColoredPrompt(typed);

        if (typed.Length >= currentPassage.Length)
            FinishGame(typed);
    }

    void BuildColoredPrompt(string typed)
    {
        string result = "";
        for (int i = 0; i < currentPassage.Length; i++)
        {
            char c = currentPassage[i];
            if (i < typed.Length)
            {
                if (typed[i] == c)
                    result += $"<color=green>{Escape(c)}</color>";
                else
                    result += $"<color=red>{Escape(c)}</color>";
            }
            else
            {
                result += $"<color=white>{Escape(c)}</color>";
            }
        }
        promptText.text = result;
    }

    void FinishGame(string typed)
    {
        finished = false;
        typingInput.interactable = false;

        float elapsed = Time.time - startTime;

        if (elapsed < 0.5f)
        {
            messageText.text = "Done! (type manually for a WPM score)";
        }
        else
        {
            float wpm = (currentPassage.Length / 5f) / (elapsed / 60f);
            messageText.text = $"Done!  {Mathf.RoundToInt(wpm)} WPM  ({elapsed:0.0}s)";
        }

        finished = true;
    }

    // Escape TMP rich-text special characters
    string Escape(char c)
    {
        return c switch
        {
            '<' => "<noparse><</noparse>",
            '>' => "<noparse>></noparse>",
            _   => c.ToString()
        };
    }
}
