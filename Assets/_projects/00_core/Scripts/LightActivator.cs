using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Merubo.Concert
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None), RequireComponent(typeof(Light))]
    public class LightActivator : MeruboUdon
    {
        private Light _lightComponent;

        void Start()
        {
            _lightComponent = GetComponent<Light>();
            if (_lightComponent == null)
            {
                Debug.LogError("Light component not found on LightActivator.");
                enabled = false; // Disable this script if no light component is found
            }
        }

        private void Update()
        {
            var intensity = _lightComponent.intensity;
            if (_lightComponent.enabled)
            {
                if (intensity < float.Epsilon)
                {
                    _lightComponent.enabled = false;
                }
            }
            else
            {
                if (intensity > float.Epsilon)
                {
                    _lightComponent.enabled = true;
                }
            }
        }
    }
}