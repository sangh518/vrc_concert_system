using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Merubo.Concert
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class SkyBoxLightFollower : MeruboUdon
    {
        [SerializeField] private Light skyColorLight, equatorColorLight, groundColorLight;

        private Color _skyColor, _equatorColor, _groundColor;


        private void Update()
        {
            bool isChanged = false;
            if (skyColorLight.color != _skyColor)
            {
                RenderSettings.ambientSkyColor = skyColorLight.color;
                _skyColor = skyColorLight.color;
                isChanged = true;
            }


            if (equatorColorLight.color != _equatorColor)
            {
                RenderSettings.ambientEquatorColor = equatorColorLight.color;
                _equatorColor = equatorColorLight.color;
                isChanged = true;
            }

            if (groundColorLight.color != _groundColor)
            {
                RenderSettings.ambientGroundColor = groundColorLight.color;
                _groundColor = groundColorLight.color;
                isChanged = true;
            }

            if (isChanged)
            {
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            }
        }
    }
}