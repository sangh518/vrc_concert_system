using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Merubo.Concert
{
    [DefaultExecutionOrder(-100), UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class OverlayImage : UdonSharpBehaviour
    {
        [SerializeField] private OverlayImageManager overlayImageManager;
        [SerializeField] private GameObject[] overlayImages;

        private void Start()
        {
            overlayImageManager.AddListener(this);
        }

        public void SetOverlayImage(int index)
        {
            for (var i = 0; i < overlayImages.Length; i++)
            {
                overlayImages[i].SetActive(i == index);
            }
        }
    }
}