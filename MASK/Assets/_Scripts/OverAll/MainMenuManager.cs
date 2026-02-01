using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("素材预留 (将来拖入按钮贴图)")]
    public Sprite startBtnSprite;
    public Sprite levelBtnSprite;
    public Sprite quitBtnSprite;


    public void StartGame()
    {
        SceneManager.LoadScene("DEMO1");
        Debug.Log("进入场景1");
    }
    public void OpenLevelSelect()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void QuitGame()
    {
        Debug.Log("退出游戏...");
        Application.Quit();
    }
}