using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Merubo.Concert
{
    [DefaultExecutionOrder(-100), UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class ISongDisplay : MeruboUdon
    {
        [SerializeField] private SongDisplaySystem songDisplaySystem;

        private void Start()
        {
            songDisplaySystem.AddListeners(this);
        }

        public virtual void SetTitle(string title, string author)
        {
        }

        public virtual void SetSubtitle(string subtitle, string subtitleOriginal)
        {
        }
    }
}