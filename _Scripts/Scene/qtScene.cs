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
        ChangeInfo = 2,
        Setting = 3,
        ConfirmPlayGame = 4,
        MergeCard = 5,
        SplitCard = 6,
    }

    public enum EScene
    {
        MainMenu = 0,
        GameScene = 1
    }

    public enum EHud
    {
        Menu = 0
    }
}