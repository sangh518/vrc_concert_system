using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Merubo.Concert
{
    public enum SubtitleType
    {
        KoreanOnly,
        Japanese,
        English,
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class SongData : MeruboUdon
    {
        public string title;
        public string author;

        public SubtitleType subtitleType = SubtitleType.KoreanOnly;
        public TextAsset subtitleCsv;
        public float startOffsetTime;


        private void Start()
        {
            if (subtitleCsv == null)
            {
                Debug.LogError($"[{nameof(SongData)}] Subtitle CSV is not set for song: {title} by {author}");
                return;
            }

            ParseData();
        }

        private void ParseData()
        {
            //TODO
        }
    }
}