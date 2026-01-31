using UnityEngine;
using UnityEngine.SceneManagement; // 必须引用

public class Door : MonoBehaviour
{
    [Header("状态")]
    public bool isOpen = false;

    [Header("跳转配置")]
    [Tooltip("填入下一关场景的名称。如果为空，则自动加载 Build Settings 中的下一个场景")]
    public string nextSceneName;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;
        Debug.Log("所有的祭坛都被激活了，门已开启！");
       
        if (sr != null) sr.color = new Color(0, 1, 0, 0.5f);
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    // 玩家进入位置时被调用
    public void EnterDoor()
    {
        if (!isOpen) return;

        Debug.Log("检测到玩家进入，准备跳转场景...");

        // 1. 优先尝试按名称跳转
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        // 2. 如果没填名字，自动跳转到 Build Settings 里的下一个索引
        else
        {
            int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextIndex);
            }
            else
            {
                Debug.LogError("跳转失败：未设置 nextSceneName 且没有后续场景！");
            }
        }
    }
}