using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Merubo.Concert
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class SongSelectButton : MeruboUdon
    {
        [SerializeField] private ConcertTimelinePlayer concertTimelinePlayer;
        [SerializeField] private SongDisplaySystem songDisplaySystem;
        [SerializeField] private int index = -1;
        [SerializeField] private TextMeshProUGUI titleText, authorText;


        private void Start()
        {
            if (index == -1)
            {
                SetButtonText("---", "---");
                return;
            }

            SongData songData = songDisplaySystem.GetSongData(index);
            if (songData == null)
            {
                SetButtonText("---", "---");
                return;
            }

            SetButtonText(songData.title, songData.author);
        }

        private void SetButtonText(string title, string author)
        {
            titleText.text = title;
            authorText.text = author;
        }

        public void OnClick()
        {
            if (index == -1) return;
            concertTimelinePlayer.PlayTimeline(index);
        }
    }
}