using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalaxyEpisode : GenericEpisode
{
    public Camera _camera;

    public override void OnLaunch() {
        if(_camera != null) {
            _camera.clearFlags = CameraClearFlags.Skybox;
        }
    }

    public override void OnStop() {
        if(_camera != null) {
            _camera.clearFlags = CameraClearFlags.SolidColor;
        }
    }
}
