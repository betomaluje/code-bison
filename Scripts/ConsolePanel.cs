using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ConsolePanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _consoleText;

    [Tooltip("Delay to clear the console in seconds")] 
    [SerializeField, Range(1, 5)]
    private int _resetTextDelay = 1;

    public static ConsolePanel Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void WriteConsole(string text)
    {
        WriteConsole(text, _resetTextDelay);
    }

    public async void WriteConsole(string text, int delay)
    {
        _consoleText.text = "";
        _consoleText.text = text;

        // we only clear when there's a delay
        if (delay <= 0) return;

        await Task.Delay(_resetTextDelay * 1000);
        _consoleText.text = "";
    }
}