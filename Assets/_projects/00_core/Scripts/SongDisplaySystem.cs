using System;
using Merubo.Concert;
using UdonSharp;
using UnityEngine;
using UnityEngine.Playables;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace Merubo.Concert
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class SongDisplaySystem : MeruboUdon
    {
        [SerializeField] private ISongDisplay[] displays;
        [SerializeField] private SongData[] songDataList;


        private PlayableDirector _currentDirector;
        private SongData _currentSongData;
        private int _currentSubtitleIndex = -1;


        private void Start()
        {
            ClearDisplay();
        }

        private void Update()
        {
            if (_currentDirector == null || _currentSongData == null) return;
            var currentTime = _currentDirector.time;

            // 현재 시간에 맞는 자막 인덱스를 찾습니다.
            int targetSubtitleIndex = -1;
            for (int i = 0; i < _currentSongData.startTimeArray.Length; i++)
            {
                if (currentTime >= _currentSongData.startTimeArray[i] && currentTime < _currentSongData.endTimeArray[i])
                {
                    targetSubtitleIndex = i;
                    break; // 찾았으면 반복 중단
                }
            }

            // 표시해야 할 자막이 변경되었을 때만 업데이트합니다.
            if (targetSubtitleIndex != _currentSubtitleIndex)
            {
                _currentSubtitleIndex = targetSubtitleIndex;

                if (_currentSubtitleIndex == -1)
                {
                    // 활성화된 자막이 없으면 자막을 지웁니다.
                    SetSubtitle("", "");
                }
                else
                {
                    // 새로운 자막을 표시합니다.
                    string subtitle = _currentSongData.koreanArray[_currentSubtitleIndex];
                    string subtitleOriginal = "";

                    // SongData의 subtitleType에 따라 원문 자막을 함께 표시할지 결정합니다.
                    switch (_currentSongData.subtitleType)
                    {
                        case SubtitleType.Japanese:
                        case SubtitleType.English:
                            subtitleOriginal = _currentSongData.originalArray[_currentSubtitleIndex];
                            break;
                        // KoreanOnly의 경우 subtitleOriginal은 빈 문자열로 둡니다.
                        case SubtitleType.KoreanOnly:
                        default:
                            break;
                    }

                    SetSubtitle(subtitle, subtitleOriginal);
                }
            }
        }


        public SongData GetSongData(int index)
        {
            if (index < 0 || index >= songDataList.Length)
            {
                return null;
            }

            return songDataList[index];
        }

        public void StartDisplay(int songIndex, PlayableDirector timeline)
        {
            _currentDirector = timeline;
            if (_currentDirector == null) return;
            if (songIndex < 0 || songIndex >= songDataList.Length)
            {
                Debug.LogError("Invalid song index: " + songIndex);
                _currentSongData = null;
                return;
            }

            _currentSongData = songDataList[songIndex];

            if (_currentSongData != null)
            {
                // 곡이 시작될 때 제목과 아티스트를 설정합니다.
                SetTitle(_currentSongData.title, _currentSongData.author);
                // 자막 인덱스를 초기화하여 첫 자막이 정상적으로 표시되도록 합니다.
                _currentSubtitleIndex = -1;
            }
            else
            {
                ClearDisplay();
            }

            SetSubtitle("", ""); // 초기 자막을 비웁니다.
        }

        public void ClearDisplay()
        {
            _currentDirector = null;
            _currentSongData = null;
            _currentSubtitleIndex = -1; // 인덱스 초기화
            SetTitle("", "");
            SetSubtitle("", "");
        }


        private void SetTitle(string title, string author)
        {
            foreach (var display in displays)
            {
                display.SetTitle(title, author);
            }
        }

        private void SetSubtitle(string subtitle, string subtitleOriginal)
        {
            foreach (var display in displays)
            {
                display.SetSubtitle(subtitle, subtitleOriginal);
            }
        }
    }
}