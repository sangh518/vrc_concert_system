using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Merubo.Concert
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class StageLightManager : MeruboUdon
    {
        #region StateIndex [UdonSynced int]

        [UdonSynced(), FieldChangeCallback(nameof(StateIndex))]
        private int _stateIndex;

        public int StateIndex
        {
            get => _stateIndex;
            set
            {
                var pre = _stateIndex;
                _stateIndex = value;
                OnSyncedStateIndexChanged(pre, value);
            }
        }

        public void SetSyncedStateIndex(int value)
        {
            if (!Networking.IsOwner(gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }

            StateIndex = value;
            RequestSerialization();
        }

        #endregion


        [SerializeField] private Animator animator;

        private void Start()
        {
            StateIndex = StateIndex;
        }

        private void OnSyncedStateIndexChanged(int pre, int value)
        {
            animator.SetInteger("index", value);
        }

        public void SetIndex0() => SetSyncedStateIndex(0);
        public void SetIndex1() => SetSyncedStateIndex(1);
        public void SetIndex2() => SetSyncedStateIndex(2);
        public void SetIndex3() => SetSyncedStateIndex(3);
        public void SetIndex4() => SetSyncedStateIndex(4);
        public void SetIndex5() => SetSyncedStateIndex(5);
        public void SetIndex6() => SetSyncedStateIndex(6);
        public void SetIndex7() => SetSyncedStateIndex(7);
        public void SetIndex8() => SetSyncedStateIndex(8);
        public void SetIndex9() => SetSyncedStateIndex(9);
        public void SetIndex10() => SetSyncedStateIndex(10);
    }
}