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
        [SerializeField] private SongData[] songDataList;

        private DataList _listeners;

        private PlayableDirector _currentDirector;

        private void Start()
        {
            ClearDisplay();
        }

        private void Update()
        {
            if (_currentDirector != null) return;

            var songData = GetSongData(0);
            if (songData == null) return;

            var currentTime = _currentDirector.time;
            //TODO
        }

        public void AddListeners(ISongDisplay listener)
        {
            if (_listeners == null)
            {
                _listeners = new DataList();
            }

            _listeners.Add(listener);
        }

        public SongData GetSongData(int index)
        {
            if (index < 0 || index >= songDataList.Length)
            {
                return null;
            }

            return songDataList[index];
        }

        public void StartDisplay(PlayableDirector timeline)
        {
            _currentDirector = timeline;
            if (_currentDirector == null) return;

            //TODO
        }

        public void ClearDisplay()
        {
            _currentDirector = null;
            SetTitle("", "");
            SetSubtitle("", "");
        }


        private void SetTitle(string title, string author)
        {
            if (_listeners == null) return;

            for (int i = 0; i < _listeners.Count; i++)
            {
                ISongDisplay listener = (ISongDisplay)_listeners[i].Reference;
                listener.SetTitle(title, author);
            }
        }

        private void SetSubtitle(string subtitle, string subtitleOriginal)
        {
            if (_listeners == null) return;

            for (int i = 0; i < _listeners.Count; i++)
            {
                ISongDisplay listener = (ISongDisplay)_listeners[i].Reference;
                listener.SetSubtitle(subtitle, subtitleOriginal);
            }
        }
    }
}