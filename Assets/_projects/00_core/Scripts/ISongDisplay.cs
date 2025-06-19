using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Merubo.Concert
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class ISongDisplay : MeruboUdon
    {
        public virtual void SetTitle(string title, string author)
        {
        }

        public virtual void SetSubtitle(string subtitle, string subtitleOriginal)
        {
        }
    }
}