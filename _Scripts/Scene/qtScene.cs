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
        Register,
        Conversation,
        Reply,
        CourseInfo,
        CreatePost,
        ReportPost,
        CreateDataLink,
        Social,
        LoginDataLink,
        ChangeName,
        SoundSetting,
        NotiSetting,
        UserInfo,
        ConfirmRule
    }

    public enum EScene
    {
        MenuScene = 0,
        GameScene = 1,
        LoginScene = 2,
        ChangeDesignScene = 3,
        ForumScene = 4,
        CourseScene = 5,
        MyPageScene = 6,
        IntroScene = 7,
        InnerChild = 8,
        LoadingScene = 9,
        SettingScene = 10,
        UserAvatarScene = 11
    }

    public enum EHud
    {
        None = 0,
        Menu = 1
    }
}