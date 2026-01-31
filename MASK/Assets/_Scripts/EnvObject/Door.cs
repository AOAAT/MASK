using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [Header("状态")]
    public bool isOpen = false;

    [Header("视觉配置")]
    public Sprite closedDoorSprite; 
    public Sprite openDoorSprite;   

    [Header("跳转配置")]
    [Tooltip("填入下一关场景的名称。如果为空，则自动加载 Build Settings 中的下一个场景")]
    public string nextSceneName;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        // 初始状态确保是关门贴图
        if (sr != null && closedDoorSprite != null)
        {
            sr.sprite = closedDoorSprite;
        }
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;
        Debug.Log("所有的祭坛都被激活了，门已开启！");

        // 1. 切换为开门贴图
        if (sr != null && openDoorSprite != null)
        {
            sr.sprite = openDoorSprite;
            sr.color = Color.white;
        }

        // 2. 修改层级使玩家可以穿过
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    // 玩家进入位置时被调用
    public void EnterDoor()
    {
        if (!isOpen) return;

        Debug.Log("检测到玩家进入，准备跳转场景...");

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
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