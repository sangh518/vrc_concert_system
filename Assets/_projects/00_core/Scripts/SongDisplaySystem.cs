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
        [SerializeField] private SongPromptDisplay[] promptDisplays;
        [SerializeField] private SongData[] songDataList;


        private PlayableDirector _currentDirector;
        private SongData _currentSongData;
        private int _currentSubtitleIndex = -1;

        private bool _hasTitleShown = false; // 제목이 이미 표시되었는지 여부

        private void Start()
        {
            ClearDisplay();
        }

        private void Update()
        {
            if (_currentDirector == null || _currentSongData == null) return;
            var currentTime = _currentDirector.time;

            if (!_hasTitleShown)
            {
                var titleTime = _currentSongData.titleOpenOffsetTime + _currentSongData.startOffsetTime;
                // 현재 시간이 제목 표시 시간보다 작으면 제목을 표시하지 않습니다.
                if (currentTime > titleTime)
                {
                    SetTitle(_currentSongData.title, _currentSongData.author);
                    _hasTitleShown = true; // 제목이 표시되었음을 기록합니다.
                }
            }


            // 현재 시간에 맞는 자막 인덱스를 찾습니다.
            int targetSubtitleIndex = -1;
            for (int i = 0; i < _currentSongData.startTimeArray.Length; i++)
            {
                if (currentTime >= _currentSongData.startTimeArray[i])
                {
                    if (currentTime < _currentSongData.endTimeArray[i])
                    {
                        targetSubtitleIndex = i;
                        break; // 찾았으면 반복 중단   
                    }
                }
            }

            // 표시해야 할 자막이 변경되었을 때만 업데이트합니다.
            if (targetSubtitleIndex != _currentSubtitleIndex)
            {
                _currentSubtitleIndex = targetSubtitleIndex;

                if (_currentSubtitleIndex == -1)
                {
                    // 활성화된 자막이 없으면 자막과 프롬프트를 지웁니다.
                    SetSubtitle("", "");
                }
                else
                {
                    // 1. 새로운 메인 자막을 표시합니다.
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

                    // 2. 프롬프트에 사용할 자막 배열을 결정합니다.
                    string[] promptSourceArray;
                    if (_currentSongData.subtitleType == SubtitleType.KoreanOnly)
                    {
                        // KoreanOnly 타입일 경우 한국어 자막을 프롬프트로 사용합니다.
                        promptSourceArray = _currentSongData.koreanArray;
                    }
                    else
                    {
                        // 그 외의 경우(Japanese, English 등) 원문 자막을 프롬프트로 사용합니다.
                        promptSourceArray = _currentSongData.promptArray;
                    }

                    // 3. 현재 프롬프트와 다음 프롬프트 텍스트를 가져옵니다.
                    string currentPrompt = promptSourceArray[_currentSubtitleIndex];
                    string nextPrompt = "";

                    // 다음 자막 인덱스를 계산합니다.
                    int nextSubtitleIndex = _currentSubtitleIndex + 1;

                    // 다음 자막이 존재하는지 확인합니다 (배열 범위 초과 방지).
                    if (nextSubtitleIndex < promptSourceArray.Length)
                    {
                        nextPrompt = promptSourceArray[nextSubtitleIndex];
                    }

                    // 4. 프롬프트 디스플레이를 업데이트합니다.
                    SetPrompt(currentPrompt, nextPrompt);
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
                SetTitle("", "");
                SetPromptTitle(_currentSongData.title, _currentSongData.author);
                // 자막 인덱스를 초기화하여 첫 자막이 정상적으로 표시되도록 합니다.
                _currentSubtitleIndex = -1;
            }
            else
            {
                ClearDisplay();
            }

            SetSubtitle("", ""); // 초기 자막을 비웁니다.

            string[] promptArray;

            if (_currentSongData.subtitleType == SubtitleType.KoreanOnly) promptArray = _currentSongData.koreanArray;
            else promptArray = _currentSongData.promptArray;

            string next = "";
            if (promptArray.Length > 0)
            {
                // 프롬프트 배열이 비어있지 않으면 첫 번째 프롬프트를 가져옵니다.
                next = promptArray[0];
            }

            SetPrompt("", next); // 초기 프롬프트도 비웁니다.
        }

        public void ClearDisplay()
        {
            _currentDirector = null;
            _currentSongData = null;
            _currentSubtitleIndex = -1; // 인덱스 초기화
            _hasTitleShown = false;
            SetTitle("", "");
            SetSubtitle("", "");
            SetPrompt("", "");
            SetPromptTitle("", "");
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

        public void SetPrompt(string prompt, string nextPrompt)
        {
            foreach (var promptDisplay in promptDisplays)
            {
                promptDisplay.SetPrompt(prompt, nextPrompt);
            }
        }

        private void SetPromptTitle(string title, string author)
        {
            foreach (var promptDisplay in promptDisplays)
            {
                promptDisplay.SetTitle(title, author);
            }
        }
    }
}