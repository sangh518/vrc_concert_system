using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Merubo.Concert
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class DefaultSongDisplay : ISongDisplay
    {
        // 제목
        [SerializeField] private Animator titleAnimator;
        [SerializeField] private TextMeshProUGUI titleText, authorText;

        // 자막
        [SerializeField] private TextMeshProUGUI subtitleText, subtitleOriginalText;

        public override void SetTitle(string title, string author)
        {
            titleText.text = title;
            authorText.text = author;

            titleText.gameObject.SetActive(title != "");
            authorText.gameObject.SetActive(author != "");

            if (title != "" && author != "")
            {
                titleAnimator.SetTrigger("Play");
            }
        }

        public override void SetSubtitle(string subtitle, string subtitleOriginal)
        {
            subtitleText.text = subtitle;
            subtitleText.gameObject.SetActive(!string.IsNullOrEmpty(subtitle));
            subtitleOriginalText.text = subtitleOriginal;
            subtitleOriginalText.gameObject.SetActive(!string.IsNullOrEmpty(subtitleOriginal));
        }
    }
}