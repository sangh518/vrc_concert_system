using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace Merubo.Concert
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class OverlayImageManager : MeruboUdon
    {
        #region OverlayImageIndex [UdonSynced int]

        [UdonSynced] [FieldChangeCallback(nameof(OverlayImageIndex))]
        private int _overlayImageIndex = -1;

        public int OverlayImageIndex
        {
            get => _overlayImageIndex;
            set
            {
                var pre = _overlayImageIndex;
                _overlayImageIndex = value;
                OnSyncedOverlayImageIndexChanged(pre, value);
            }
        }

        public void SetSyncedOverlayImageIndex(int value)
        {
            if (!Networking.IsOwner(gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);

            OverlayImageIndex = value;
            RequestSerialization();
        }

        private void OnSyncedOverlayImageIndexChanged(int pre, int value)
        {
            foreach (var overlayImage in overlayImages)
            {
                overlayImage.SetOverlayImage(value);
            }
        }

        #endregion

        [SerializeField] private BroadCamSystem.BroadCamSystem broadCamSystem;
        [SerializeField] private OverlayImage[] overlayImages;
        [SerializeField] private bool useKeypadControl = true;

        [SerializeField] private KeyCode keycodeOff, keycode1, keycode2;


        private void Start()
        {
            OverlayImageIndex = OverlayImageIndex;
        }

        private void Update()
        {
            if (!useKeypadControl) return;
            if (!broadCamSystem.KeypadControlEnable) return;

            if (Input.GetKeyDown(keycodeOff))
            {
                SetOverlayImageIndex(-1);
            }
            else if (Input.GetKeyDown(keycode1))
            {
                SetOverlayImageIndex(0);
            }
            else if (Input.GetKeyDown(keycode2))
            {
                SetOverlayImageIndex(1);
            }
        }


        public void SetOverlayImageIndex(int index)
        {
            if (OverlayImageIndex == index) SetSyncedOverlayImageIndex(-1);
            else SetSyncedOverlayImageIndex(index);
        }
    }
}