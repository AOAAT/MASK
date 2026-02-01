using UnityEngine;
using TMPro;

public class LevelButton : MonoBehaviour
{
    private string sceneName;

    // 由管理器调用进行初始化
    public void Setup(int levelIndex, string name)
    {
        sceneName = name;
        GetComponentInChildren<TextMeshProUGUI>().text = levelIndex.ToString();
    }

    // 按钮点击事件
    public void OnClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}