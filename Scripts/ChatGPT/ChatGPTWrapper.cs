using System;
using System.Text.RegularExpressions;
using ChatGPTWrapper;
using TMPro;
using UnityEngine;

namespace BerserkPixel.ChatGPT
{
    public class ChatGPTWrapper : MonoBehaviour
    {
        [SerializeField] private ChatGPTConversation _chatGptConversation;
        [SerializeField] private TMP_InputField _promptField;
        [SerializeField] private TMP_InputField _finalPromptField;

        public void Editor_ChangeInitialPrompt(string newPrompt)
        {
            if (string.IsNullOrEmpty(newPrompt)) return;
            
            _chatGptConversation.ResetChat(newPrompt);
        }

        public void Editor_UpdateFinalPrompt(string newPrompt)
        {
            LoadingPanel.LoadingEndEvent.Invoke();
            
            if (string.IsNullOrEmpty(newPrompt)) return;
            
            newPrompt = ToUTF32(newPrompt);
            _finalPromptField.text = newPrompt;
        }
        
        private string ToUTF32(string input)
        {
            string output = input;
            Regex pattern = new Regex(@"\\u[a-zA-Z0-9]*");

            while (output.Contains(@"\u"))
            {
                output = pattern.Replace(output, @"\U000" + output.Substring(output.IndexOf(@"\u", StringComparison.Ordinal) + 2, 5), 1);
            }

            return output;
        }
        
        public void Click_SendToChatGPT()
        {
            if (string.IsNullOrEmpty(_promptField.text))
            {
                ConsolePanel.Instance.WriteConsole("You need to write a proper prompt first", -1);
                return;
            }
            
            LoadingPanel.LoadingStartEvent.Invoke();
            
            _chatGptConversation.SendToChatGPT(_promptField.text);
        }
    }
}