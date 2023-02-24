using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class qtScene
{
    private static SceneData _sceneData;
    public static SceneData sceneData
    {
        get
        {
            if (_sceneData == null)
            {
                _sceneData = Resources.Load<SceneData>("_Data/SceneConfig");
            }

            return _sceneData;
        }
    }
    public enum EPopup
    {
        Noti = 0,
        YesNo = 1,
    }

    public enum EScene
    {
        MenuScene = 0,
    }

    public enum EHud
    {
        None = 0,
        Menu = 1
    }
}