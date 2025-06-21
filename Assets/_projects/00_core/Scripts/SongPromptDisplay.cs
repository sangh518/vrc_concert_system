using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Merubo.Concert
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class SongPromptDisplay : MeruboUdon
    {
        [SerializeField] private TextMeshProUGUI promptText, nextPromptText, titleAuthorText;

        public void SetPrompt(string prompt, string promptNext)
        {
            promptText.text = prompt;
            nextPromptText.text = promptNext;
        }

        public void SetTitle(string title, string author)
        {
            titleAuthorText.text = $"{title} - {author}";
        }
    }
}