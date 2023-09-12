using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectModelPanel : View
{

    // 点击模式选择按钮
    public void OnSelectModelClick( int count )
    {
        // 选择模式
        PlayerPrefs.SetInt(Const.GameModel, count);
        // 跳转场景 到 游戏场景
        SceneManager.LoadSceneAsync(1);
        // asyncOperation.progress

    }


}
