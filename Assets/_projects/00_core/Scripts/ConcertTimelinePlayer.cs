using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.Playables;
using VRC.SDKBase;
using VRC.Udon;

namespace Merubo.Concert
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ConcertTimelinePlayer : MeruboUdon
    {
        #region TimelineIndex [UdonSynced int]

        [UdonSynced(), FieldChangeCallback(nameof(TimelineIndex))]
        private int _timelineIndex = -1;

        public int TimelineIndex
        {
            get => _timelineIndex;
            set
            {
                var pre = _timelineIndex;
                _timelineIndex = value;
                OnSyncedTimelineIndexChanged(pre, value);
            }
        }

        public void SetSyncedTimelineIndex(int value)
        {
            if (!Networking.IsOwner(gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }

            TimelineIndex = value;
            RequestSerialization();
        }

        private void OnSyncedTimelineIndexChanged(int pre, int value)
        {
            if (value < 0)
            {
                DebugLog("Stopping timeline playback.");
                return;
            }

            DebugLog("Playing timeline at index: " + value);

            PlayableDirector targetTimeline = null;

            for (var i = 0; i < timelines.Length; i++)
            {
                if (i == value)
                {
                    timelines[i].Play();
                    targetTimeline = timelines[i];
                }
                else
                {
                    timelines[i].Stop();
                }
            }

            if (targetTimeline != null)
            {
                songDisplaySystem.StartDisplay(targetTimeline);
            }
            else
            {
                songDisplaySystem.ClearDisplay();
            }
        }

        #endregion


        [SerializeField] private SongDisplaySystem songDisplaySystem;
        [SerializeField] private PlayableDirector[] timelines;

        private void Start()
        {
            TimelineIndex = TimelineIndex;
        }

        public void PlayTimeline(int index)
        {
            if (index < 0) return;
            SetSyncedTimelineIndex(index);
        }

        public void StopTimeline() => SetSyncedTimelineIndex(-1);
    }
}