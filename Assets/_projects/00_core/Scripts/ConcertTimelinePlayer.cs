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
                songDisplaySystem.StartDisplay(value, targetTimeline);
                IsPlaying = true;
            }
            else
            {
                songDisplaySystem.ClearDisplay();
                IsPlaying = false;
            }
        }

        #endregion


        [SerializeField] private SongDisplaySystem songDisplaySystem;
        [SerializeField] private PlayableDirector[] timelines;

        [Header("Shift + [ ]로 타임라인 스킵(로컬), 테스트용")] [SerializeField]
        private bool useSkipKey = false;

        private void Start()
        {
            TimelineIndex = TimelineIndex;
        }

        private bool _isPlaying;

        private bool IsPlaying
        {
            set
            {
                if (_isPlaying == value) return;
                _isPlaying = value;
                OnTimelinePlayStateChanged(!_isPlaying, _isPlaying);
            }
            get => _isPlaying;
        }

        private void Update()
        {
            if (timelines == null || timelines.Length == 0) return;

            if (_timelineIndex < 0 || _timelineIndex >= timelines.Length)
            {
                return;
            }

            var currentTimeline = timelines[_timelineIndex];

            if (currentTimeline.time + Double.Epsilon >= currentTimeline.duration && IsPlaying)
            {
                StopTimeline();
            }

            if (useSkipKey)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (Input.GetKeyDown(KeyCode.LeftBracket))
                    {
                        currentTimeline.time = Mathf.Clamp((float)currentTimeline.time - 5f, 0f,
                            (float)currentTimeline.duration);
                    }
                    else if (Input.GetKeyDown(KeyCode.RightBracket))
                    {
                        currentTimeline.time = Mathf.Clamp((float)currentTimeline.time + 5f, 0f,
                            (float)currentTimeline.duration);
                    }
                }
            }
        }

        public void PlayTimeline(int index)
        {
            if (index < 0) return;
            SetSyncedTimelineIndex(index);
        }

        public void StopTimeline() => SetSyncedTimelineIndex(-1);


        private void OnTimelinePlayStateChanged(bool pre, bool value)
        {
            DebugLog("Timeline play : " + value);
        }
    }
}