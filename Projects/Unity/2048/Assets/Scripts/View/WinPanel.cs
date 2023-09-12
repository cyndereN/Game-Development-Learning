using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinPanel :  View
{

    // 重新开始的按钮点击事件
    public void OnRestartClick() {
        // 调用GamePanel 里面的重新开始

        GameObject.Find("Canvas/GamePanel").GetComponent<GamePanel>().RestartGame();
        Hide();
    }

    // 退出按钮的点击事件
    public void OnExitClick() {
        // 退出到菜单场景
        SceneManager.LoadSceneAsync(0);
    }

}
