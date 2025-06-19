using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.Serialization;
using VRC.SDK3.Data;
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

    // csv 포맷
    // index	start_time	end_time	korean	original	prompt


    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class SongData : MeruboUdon
    {
        public string title;
        public string author;

        public SubtitleType subtitleType = SubtitleType.KoreanOnly;
        public TextAsset subtitleCsv;
        public float startOffsetTime;


        [HideInInspector] public float[] startTimeArray, endTimeArray;
        [HideInInspector] public string[] koreanArray, originalArray, promptArray;

        private void OnEnable()
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
            string[] lines = subtitleCsv.text.Split('\n');

            startTimeArray = new float[lines.Length - 1];
            endTimeArray = new float[lines.Length - 1];
            koreanArray = new string[lines.Length - 1];
            originalArray = new string[lines.Length - 1];
            promptArray = new string[lines.Length - 1];


            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0) continue;

                var line = lines[i];

                if (line == lines[0]) continue;

                string[] parts = line.Split('\t');
                float startTime = float.Parse(parts[1]) + startOffsetTime;
                startTimeArray[i - 1] = startTime;

                float endTime;
                if (parts[2] == "")
                {
                    if (i < lines.Length - 1)
                    {
                        var nextLine = lines[i + 1];
                        var nextParts = nextLine.Split('\t');
                        var nextStartTime = float.Parse(nextParts[1]) + startOffsetTime;
                        endTime = nextStartTime;
                    }
                    else
                    {
                        endTime = float.MaxValue;
                    }
                }
                else endTime = float.Parse(parts[2]) + startOffsetTime;

                endTimeArray[i - 1] = endTime;

                koreanArray[i - 1] = parts[3];
                originalArray[i - 1] = parts.Length > 4 ? parts[4] : "";
                promptArray[i - 1] = parts.Length > 5 ? parts[5] : "";
            }
        }
    }
}